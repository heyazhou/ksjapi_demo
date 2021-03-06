﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KSJCamera;
using KSJApi_TriggerMode;
using KSJ_API3A;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.IO;
using KSJ_GS;
using KSJ_FUNCTION;
using KSJ_Code;
using KSJApi_Callback;
using KSJ_Win;
//using HalconDotNet;
//using System.Drawing.Imaging; 

namespace KSJDemoCSharp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            m_nDeviceNum = 0;
            m_nDeviceCurSel = -1;
            // 初始化动态库
            KSJApiBase.KSJ_Init();
            previewCallBack = new KSJApiCallback.KSJ_PREVIEWCALLBACK(PreviewCallback);
            previewCallBackEx = new KSJApiCallback.KSJ_PREVIEWCALLBACKEX(PreviewCallbackEx);

            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                ListView_FUNCTION.View = View.Details;
                //添加列  
                ListView_FUNCTION.Columns.Add("Function", 300, HorizontalAlignment.Left);
                ListView_FUNCTION.Columns.Add("Support", 100, HorizontalAlignment.Left);

                UpdateDeviceList();
                Timer_GET_FRAME_RATE.Interval = 1000;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void UpdateDeviceList()
        {
            KSJApiBase.KSJ_UnInit();
            KSJApiBase.KSJ_Init();
            ComboBox_DEVICE_LIST.Items.Clear();
            m_nDeviceNum = KSJApiBase.KSJ_DeviceGetCount();
            Label__DEVICENUM.Text = String.Format("{0} Device Found:", m_nDeviceNum);

            if (m_nDeviceNum == 0)
            {
                m_nDeviceCurSel = -1;
                return;
            }

            if (m_nDeviceCurSel >= m_nDeviceNum)
            {
                m_nDeviceCurSel = 0;
            }

            for (int i = 0; i < m_nDeviceNum; i++)
            {
                m_DeviceInfo[i].nIndex = i;
                KSJApiBase.KSJ_DeviceGetInformationEx(i, ref m_DeviceInfo[i].DeviceType, ref m_DeviceInfo[i].nSerials, ref m_DeviceInfo[i].wFirmwareVersion, ref m_DeviceInfo[i].wFpgaVersion);
                byte btMajVersion =(byte)((m_DeviceInfo[i].wFirmwareVersion & 0xFF00) >> 8);		// 得到主版本号
                byte btMinVersion = (byte)(m_DeviceInfo[i].wFirmwareVersion & 0x00FF);				// 得到副版本号

                byte btFpgaMajVersion = (byte)((m_DeviceInfo[i].wFpgaVersion & 0xFF00) >> 8);		// 得到主版本号
                byte btFpgaMinVersion = (byte)(m_DeviceInfo[i].wFpgaVersion & 0x00FF);				// 得到副版本号

                string szText = String.Format("Index({0})-Type({1})-Serials({2})-FwVer({3}.{4})-FpgaVer({5}.{6})",
                                        i, KSJGS.g_szDeviceType[(int)(m_DeviceInfo[i].DeviceType)], m_DeviceInfo[i].nSerials, btMajVersion, btMinVersion, btFpgaMajVersion, btFpgaMinVersion);
                ComboBox_DEVICE_LIST.Items.Insert(i, szText);
            }
        }

        public void UpdateInterface()
        {
            if (m_nDeviceCurSel == -1) return;

            // Initial Exposure Time
            int nMin = 0;
            int nMax = 0;
            int nCur = 0;

            KSJApiBase.KSJ_GetParamRange(m_nDeviceCurSel, KSJApiBase.KSJ_PARAM.KSJ_EXPOSURE, ref nMin, ref nMax);
            KSJApiBase.KSJ_GetParam(m_nDeviceCurSel, KSJApiBase.KSJ_PARAM.KSJ_EXPOSURE, ref nCur);
            NumericUpDown_EXPOSURE_TIME_MS.Minimum = nMin;
            NumericUpDown_EXPOSURE_TIME_MS.Maximum = nMax;
            NumericUpDown_EXPOSURE_TIME_MS.Value = nCur;
            string szText = String.Format("{0}-{1} ms ({2}sec, {3}min)", nMin, nMax, (float)nMin / 1000.0f, (float)nMax / 60000.0f);
            Label_EXPOSURE_TIME_RANGE.Text = szText;

            KSJApiBase.KSJ_GetParamRange(m_nDeviceCurSel, KSJApiBase.KSJ_PARAM.KSJ_EXPOSURE_LINES, ref nMin, ref nMax);
            KSJApiBase.KSJ_GetParam(m_nDeviceCurSel, KSJApiBase.KSJ_PARAM.KSJ_EXPOSURE_LINES, ref nCur);
            NumericUpDown_EXPOSURE_LINES.Minimum = nMin;
            NumericUpDown_EXPOSURE_LINES.Maximum = nMax;
            NumericUpDown_EXPOSURE_LINES.Value = nCur;
            szText = String.Format("{0}-{1} Lines", nMin, nMax);
            Label_EXPOSURE_LINES_RANGE.Text = szText;

            // GAIN, Because R,G,B Gain has same range.
            KSJApiBase.KSJ_GetParamRange(m_nDeviceCurSel, KSJApiBase.KSJ_PARAM.KSJ_RED, ref nMin, ref nMax);
            KSJApiBase.KSJ_GetParam(m_nDeviceCurSel, KSJApiBase.KSJ_PARAM.KSJ_RED, ref nCur);
            NumericUpDown_GAIN.Minimum = nMin;
            NumericUpDown_GAIN.Maximum = nMax;
            NumericUpDown_GAIN.Value = nCur;
            szText = String.Format("{0}-{1} Lines", nMin, nMax);
            Label_GAIN_RANGE.Text = szText;

            // FOV
            int nColStart = 0;
            int nRowStart = 0;
            int nColSize = 0;
            int nRowSize = 0;
            KSJApiBase.KSJ_ADDRESSMODE ColAddressMode = KSJApiBase.KSJ_ADDRESSMODE.KSJ_SKIPNONE;
            KSJApiBase.KSJ_ADDRESSMODE RowAddressMode = KSJApiBase.KSJ_ADDRESSMODE.KSJ_SKIPNONE;
            KSJApiBase.KSJ_PreviewGetDefaultFieldOfView(m_nDeviceCurSel, ref nColStart, ref nRowStart, ref nColSize, ref nRowSize, ref ColAddressMode, ref RowAddressMode);
            // Preview, Capture they are same, so you should get one is ok!
            szText = String.Format("{0}-{1} Lines", nColSize, nRowSize);
            Label_FOV_RANGE.Text = szText;

            TextBox_PREVIEW_COL_START.Text = nColStart.ToString();
            TextBox_PREIVEW_ROW_START.Text = nRowStart.ToString();
            TextBox_PREIVEW_COL_SIZE.Text = nColSize.ToString();
            TextBox_PREIVEW_ROW_SIZE.Text = nRowSize.ToString();

            KSJApiBase.KSJ_CaptureGetDefaultFieldOfView(m_nDeviceCurSel, ref nColStart, ref nRowStart, ref nColSize, ref nRowSize, ref ColAddressMode, ref RowAddressMode);
            TextBox_CAPTURE_COL_START.Text = nColStart.ToString();
            TextBox_CAPTURE_ROW_START.Text = nRowStart.ToString();
            TextBox_CAPTURE_COL_SIZE.Text = nColSize.ToString();
            TextBox_CAPTURE_ROW_SIZE.Text = nRowSize.ToString();
        }
        public void UpdateInterfaceFunction()
        {
            if (m_nDeviceCurSel == -1) return;
            ListView_FUNCTION.Items.Clear();

            int i = KSJGS.g_nFunction - 1;
            string szSupport;
            int nSupport = 0;

            for (i = 0; i < KSJGS.g_nFunction; i++)
            {
                KSJFUNCTION.KSJ_QueryFunction(m_nDeviceCurSel, (KSJFUNCTION.KSJ_FUNCTION)i, ref nSupport);
                szSupport = String.Format("{0}", nSupport);
                var item = new ListViewItem();
                item.ImageIndex = i + 1;
                item.Text = KSJGS.g_szFunction[i];
                item.SubItems.Add(szSupport);
                ListView_FUNCTION.BeginUpdate();
                ListView_FUNCTION.Items.Add(item);
                ListView_FUNCTION.EndUpdate();
            }
        }

        public DEVICEINFO[] m_DeviceInfo = new DEVICEINFO[m_nMaxDevice];
        public const int m_nMaxDevice = 64;
        public struct DEVICEINFO
        {
            public int nIndex;
            public KSJApiBase.KSJ_DEVICETYPE DeviceType;
            public int nSerials;
            public ushort wFirmwareVersion;
            public ushort wFpgaVersion;
        };

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                KSJApiBase.KSJ_PreviewStart(0, false);
                KSJApiBase.KSJ_UnInit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            
            Application.Exit();
        }

        private void PrintErrorMessage(int nRet)
        {
            IntPtr pszErrorMessage = Marshal.AllocCoTaskMem(256);
            KSJApiBase.KSJ_GetErrorInfo(nRet, pszErrorMessage);
            string strErrorMessage = Marshal.PtrToStringAnsi(pszErrorMessage);
            TextBox_ERROR_INFO.Text = strErrorMessage;
            Marshal.FreeCoTaskMem(pszErrorMessage);

        }
        private int  m_nDeviceNum;
        private int m_nDeviceCurSel;

        private void TextBox_PREVIEW_COL_START_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键  
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字  
                {
                    e.Handled = true;
                }
            }  
        }

        private void TextBox_PREIVEW_ROW_START_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键  
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字  
                {
                    e.Handled = true;
                }
            }  
        }

        private void TextBox_PREIVEW_ROW_SIZE_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键  
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字  
                {
                    e.Handled = true;
                }
            }  
        }

        private void TextBox_PREIVEW_COL_SIZE_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键  
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字  
                {
                    e.Handled = true;
                }
            }  
        }

        private void TextBox_CAPTURE_ROW_START_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键  
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字  
                {
                    e.Handled = true;
                }
            }  
        }

        private void TextBox_CAPTURE_COL_START_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键  
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字  
                {
                    e.Handled = true;
                }
            }  
        }

        private void textBox10_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键  
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字  
                {
                    e.Handled = true;
                }
            }  
        }

        private void TextBox_CAPTURE_COL_SIZE_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键  
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字  
                {
                    e.Handled = true;
                }
            }  
        }

        private void ComboBox_DEVICE_LIST_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_nDeviceCurSel = ComboBox_DEVICE_LIST.SelectedIndex;
            UpdateInterface();
            UpdateInterfaceFunction();
            UpdateTrigger();
        }

        private void Button_PREVIEW_FOV_SET_Click(object sender, EventArgs e)
        {
            int nColStart = 0;
            int nRowStart = 0;
            int nColSize = 0;
            int nRowSize = 0;

            KSJApiBase.KSJ_ADDRESSMODE ColAddressMode = KSJApiBase.KSJ_ADDRESSMODE.KSJ_SKIPNONE;
            KSJApiBase.KSJ_ADDRESSMODE RowAddressMode = KSJApiBase.KSJ_ADDRESSMODE.KSJ_SKIPNONE;

            nColStart = Convert.ToInt32(TextBox_PREVIEW_COL_START.Text);
            nRowStart = Convert.ToInt32(TextBox_PREIVEW_ROW_START.Text);
            nColSize = Convert.ToInt32(TextBox_PREIVEW_COL_SIZE.Text);
            nRowSize = Convert.ToInt32(TextBox_PREIVEW_ROW_SIZE.Text);

            KSJApiBase.KSJ_PreviewSetFieldOfView(m_nDeviceCurSel, nColStart, nRowStart, nColSize, nRowSize, ColAddressMode, RowAddressMode);

            // User may not set FOV correctly, ksjapi will correct it!
            KSJApiBase.KSJ_PreviewGetFieldOfView(m_nDeviceCurSel, ref nColStart, ref nRowStart, ref nColSize, ref nRowSize, ref ColAddressMode, ref RowAddressMode);
            TextBox_PREVIEW_COL_START.Text = nColStart.ToString();
            TextBox_PREIVEW_ROW_START.Text = nRowStart.ToString();
            TextBox_PREIVEW_COL_SIZE.Text = nColSize.ToString();
            TextBox_PREIVEW_ROW_SIZE.Text = nRowSize.ToString();
        }

        private void Button_CAPTURE_FOV_SET_Click(object sender, EventArgs e)
        {
            int nColStart = 0;
            int nRowStart = 0;
            int nColSize = 0;
            int nRowSize = 0;

            KSJApiBase.KSJ_ADDRESSMODE ColAddressMode = KSJApiBase.KSJ_ADDRESSMODE.KSJ_SKIPNONE;
            KSJApiBase.KSJ_ADDRESSMODE RowAddressMode = KSJApiBase.KSJ_ADDRESSMODE.KSJ_SKIPNONE;

            nColStart = Convert.ToInt32(TextBox_CAPTURE_COL_START.Text);
            nRowStart = Convert.ToInt32(TextBox_CAPTURE_ROW_START.Text);
            nColSize = Convert.ToInt32(TextBox_CAPTURE_COL_SIZE.Text);
            nRowSize = Convert.ToInt32(TextBox_CAPTURE_ROW_SIZE.Text);

            KSJApiBase.KSJ_CaptureSetFieldOfView(m_nDeviceCurSel, nColStart, nRowStart, nColSize, nRowSize, ColAddressMode, RowAddressMode);

            // User may not set FOV correctly, ksjapi will correct it!
            KSJApiBase.KSJ_CaptureGetFieldOfView(m_nDeviceCurSel, ref nColStart, ref nRowStart, ref nColSize, ref nRowSize, ref ColAddressMode, ref RowAddressMode);
            TextBox_PREVIEW_COL_START.Text = nColStart.ToString();
            TextBox_PREIVEW_ROW_START.Text = nRowStart.ToString();
            TextBox_PREIVEW_COL_SIZE.Text = nColSize.ToString();
            TextBox_PREIVEW_ROW_SIZE.Text = nRowSize.ToString();
        }

        private void NumericUpDown_EXPOSURE_TIME_MS_ValueChanged(object sender, EventArgs e)
        {
            if (m_nDeviceCurSel == -1) return;
            int m_dwExposureTimeMs = (int)(NumericUpDown_EXPOSURE_TIME_MS.Value);
            KSJApiBase.KSJ_SetParam(m_nDeviceCurSel, KSJApiBase.KSJ_PARAM.KSJ_EXPOSURE, m_dwExposureTimeMs);

            GetRealExposureTime();
        }

        public void GetRealExposureTime()
        {
            float fExposureTimeMs = 0;
            KSJApiBase.KSJ_ExposureTimeGet(m_nDeviceCurSel, ref fExposureTimeMs);
            string szText = fExposureTimeMs.ToString("0.00000");
            Label_EXPOSURE_TIME_REAL.Text = string.Format("Exp Time: {0} ms", szText);
        }

        private void NumericUpDown_EXPOSURE_LINES_ValueChanged(object sender, EventArgs e)
        {
            if (m_nDeviceCurSel == -1) return;
            int m_dwExposureLines = (int)(NumericUpDown_EXPOSURE_LINES.Value);
            KSJApiBase.KSJ_SetParam(m_nDeviceCurSel, KSJApiBase.KSJ_PARAM.KSJ_EXPOSURE_LINES, m_dwExposureLines);

            GetRealExposureTime();
        }

        private void NumericUpDown_GAIN_ValueChanged(object sender, EventArgs e)
        {
            if (m_nDeviceCurSel == -1) return;
            int nGain = (int)(NumericUpDown_GAIN.Value);

            KSJApiBase.KSJ_SetParam(m_nDeviceCurSel, KSJApiBase.KSJ_PARAM.KSJ_RED, nGain);
            KSJApiBase.KSJ_SetParam(m_nDeviceCurSel, KSJApiBase.KSJ_PARAM.KSJ_GREEN, nGain);
            KSJApiBase.KSJ_SetParam(m_nDeviceCurSel, KSJApiBase.KSJ_PARAM.KSJ_BLUE, nGain);
        }

        private void CheckBox_PREVIEWSTART_CheckedChanged(object sender, EventArgs e)
        {
            KSJApiBase.KSJ_PreviewSetPos(m_nDeviceCurSel, PictureBox_PREVIEWWND.Handle, 0, 0, PictureBox_PREVIEWWND.Width, PictureBox_PREVIEWWND.Height);
            bool bCheck = CheckBox_PREVIEWSTART.Checked;
            KSJApiBase.KSJ_PreviewStart(m_nDeviceCurSel, (bCheck ? true : false));

            if (bCheck)
            {
                Timer_GET_FRAME_RATE.Start();
            }
            else
            {
                Timer_GET_FRAME_RATE.Stop();
            }
        }

        private void Timer_GET_FRAME_RATE_Tick(object sender, EventArgs e)
        {
            float fFrameRate = 0.0f;
            KSJApiBase.KSJ_PreviewGetFrameRate(m_nDeviceCurSel, ref fFrameRate);
            string szFrameRate = fFrameRate.ToString("0.00");
            this.Text = string.Format("KSJDemo Fps={0}", szFrameRate);
        }

        unsafe private void Button_CAPTURE_Click(object sender, EventArgs e)
        {
            if (m_nDeviceCurSel == -1) return;
            int nCaptureWidth = 0;
            int nCaptureHeight = 0;
            int nCaptureBitCount = 0;
            KSJApiTriggerMode.KSJ_TriggerModeSet(m_nDeviceCurSel, KSJApiTriggerMode.KSJ_TRIGGERMODE.KSJ_TRIGGER_SOFTWARE);
            int nRet = KSJApiBase.KSJ_CaptureGetSizeEx(m_nDeviceCurSel, ref nCaptureWidth, ref nCaptureHeight, ref nCaptureBitCount);
            PrintErrorMessage(nRet);

            byte[] pImageData = new byte[nCaptureWidth * nCaptureHeight * (nCaptureBitCount >> 3)];

            long counterStart = 0;
            KSJWin.QueryPerformanceCounter(ref counterStart);

            nRet = KSJApiBase.KSJ_CaptureRgbData(m_nDeviceCurSel, pImageData);
            PrintErrorMessage(nRet);
            if (nRet != KSJCode.RET_SUCCESS)
            {
                TextBox_ELAPSE_TIME.Text = "Capture Fail.";
            }

            long counterEnd = 0;
            KSJWin.QueryPerformanceCounter(ref counterEnd);

            long nFreq = 0;
            KSJWin.QueryPerformanceFrequency(ref nFreq);

            float fInterval = (float)(counterEnd - counterStart);
            float fElapse = fInterval / (float)nFreq * 1000;    // MS

            bool bCheck = CheckBox_SAVE.Checked;
            if (bCheck)
            {
                string szFileName = string.Format("capture-{0}-{1}-{2}-{3}-{4}-{5}-{6}-{7}.bmp", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                    DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond, fElapse);
                KSJApiBase.KSJ_HelperSaveToBmp(pImageData, nCaptureWidth, nCaptureHeight, nCaptureBitCount, szFileName);
            }

            TextBox_ELAPSE_TIME.Text = string.Format("Elapse: {0}ms", fElapse);

            Bitmap bitmap = BytesToBitmap(pImageData, nCaptureWidth, nCaptureHeight, nCaptureBitCount);
            PictureBox_PREVIEWWND.Image = bitmap;
        }

        private void PreviewCallback(IntPtr pImageData, int width, int height, int nBitCount, IntPtr lpContext)
        {
            DrawCrossLine(width / 2, height / 2, 10, pImageData, width, height, nBitCount);
        }

        unsafe private void DrawCrossLine(int nX, int nY, int nLineWidth, IntPtr pImageData, int nWidth, int nHeight, int nBitCount)
        {
            byte* pBitmap = (byte*)pImageData.ToPointer();

            int nPixelBytes = nBitCount >> 3;
            int nWidthBytes = nWidth * nPixelBytes;

            int nLineWidthPixel = (nLineWidth << 1) + 1;

            int nStartPixelH = nX - nLineWidth;
            int nStartPixelV = nY - nLineWidth;

            if (nStartPixelH < 0) nStartPixelH = 0;
            if (nStartPixelV < 0) nStartPixelV = 0;

            int nEndPixelH = nX + nLineWidth + 1;
            int nEndPixelV = nY + nLineWidth + 1;

            if (nEndPixelH > nWidth) nStartPixelH = nWidth - 1 - nLineWidthPixel;
            if (nEndPixelV > nHeight) nStartPixelV = nHeight - 1 - nLineWidthPixel;

            int i = 0;
            int j = 0;

            for (i = 0; i < nHeight; i++)     // draw vertical line
            {
                int ntemp = nWidthBytes * i + nStartPixelH * nPixelBytes;

                for (j = nStartPixelH; j <= nEndPixelH; j++)
                {
                    if (1 == nPixelBytes)
                    {
                        *(pBitmap + ntemp) = 0;
                    }
                    else
                    {
                        *(pBitmap + ntemp) = 0;
                        *(pBitmap + ntemp + 1) = 0;
                        *(pBitmap + ntemp + 2) = 255;
                    }

                    ntemp += nPixelBytes;
                }
            }

            for (i = nStartPixelV; i <= nEndPixelV; i++)// draw horizontal line
            {
                int ntemp = (nHeight - 1 - i) * nWidthBytes;
                for (j = 0; j < nWidth; j++)
                {
                    if (1 == nPixelBytes)
                    {
                        *(pBitmap + ntemp) = 0;
                    }
                    else
                    {
                        *(pBitmap + ntemp) = 0;
                        *(pBitmap + ntemp + 1) = 0;
                        *(pBitmap + ntemp + 2) = 255;
                    }

                    ntemp += nPixelBytes;
                }
            }
        }

        public static unsafe System.Drawing.Bitmap BytesToBitmap(byte[] bytes, int width, int height, int nBitCount)
        {
            try
            {
                PixelFormat bitmaptype = PixelFormat.Format8bppIndexed;
                int nPixelBytes = nBitCount >> 3;

                if (3 == nPixelBytes)
                {
                    bitmaptype = PixelFormat.Format24bppRgb;
                }
                else if (4 == nPixelBytes)
                {
                    bitmaptype = PixelFormat.Format32bppPArgb;
                }

                Bitmap bitmap = new Bitmap(width, height, bitmaptype);
                //获取图像的BitmapData对像 
                BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmaptype);

                byte* ptr = (byte*)(bmpData.Scan0);
                int nWidthBytes = width * nPixelBytes;
                int nSrcOffset = 0;
                int nDesOffset = 0;
                for (int i = 0; i < height; i++)
                {
                    nSrcOffset = (height - i - 1) * nWidthBytes;
                    nDesOffset = 0;
                    for (int j = 0; j < width; j++)
                    {
                        for (int k = 0; k < nPixelBytes; k++)
                        {
                            *(ptr + nDesOffset + k) = bytes[nSrcOffset + k];
                        }

                        nDesOffset += nPixelBytes;
                        nSrcOffset += nPixelBytes;
                    }

                    ptr = ptr + nWidthBytes;
                }

                bitmap.UnlockBits(bmpData);  // 解锁内存区域

                if (1 == nPixelBytes)
                {
                    // 修改生成位图的索引表，从伪彩修改为灰度
                    ColorPalette palette;
                    // 获取一个Format8bppIndexed格式图像的Palette对象
                    using (Bitmap bmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed))
                    {
                        palette = bmp.Palette;
                    }
                    for (int i = 0; i < 256; i++)
                    {
                        palette.Entries[i] = Color.FromArgb(i, i, i);
                    }
                    // 修改生成位图的索引表
                    bitmap.Palette = palette;
                }

                return bitmap;
            }
            catch (ArgumentNullException ex)
            {
                throw ex;
            }
            catch (ArgumentException ex)
            {
                throw ex;
            }
        }

        private void PreviewCallbackEx(IntPtr hDC, IntPtr returnBitmap, int width, int height, int nBitCount, IntPtr lpContext)
        {
            DateTime dt = DateTime.Now;
            Graphics g = Graphics.FromHdc(hDC);
            Font f = new Font("宋体", height / 20, FontStyle.Regular);
            g.DrawString(dt.ToString(), f, Brushes.AliceBlue, width / 20, height / 14);
            g.Dispose();
        }

        KSJApiCallback.KSJ_PREVIEWCALLBACK previewCallBack;
        KSJApiCallback.KSJ_PREVIEWCALLBACKEX previewCallBackEx;
        private void CheckBox_SET_CALLBACK_CheckedChanged(object sender, EventArgs e)
        {
            bool bCheck = CheckBox_SET_CALLBACK.Checked;
            if (bCheck)
            {
                KSJApiCallback.KSJ_PreviewSetCallback(m_nDeviceCurSel, previewCallBack, PictureBox_PREVIEWWND.Handle);
            }
            else
            {
                KSJApiCallback.KSJ_PreviewSetCallback(m_nDeviceCurSel, null, PictureBox_PREVIEWWND.Handle);
            }
        }

        private void CheckBox_SET_CALLBACKEX_CheckedChanged(object sender, EventArgs e)
        {
            bool bCheck = CheckBox_SET_CALLBACKEX.Checked;
            if (bCheck)
            {
                KSJApiCallback.KSJ_PreviewSetCallbackEx(m_nDeviceCurSel, previewCallBackEx, PictureBox_PREVIEWWND.Handle);
            }
            else
            {
                KSJApiCallback.KSJ_PreviewSetCallbackEx(m_nDeviceCurSel, null, PictureBox_PREVIEWWND.Handle);
            }
        }

        KSJApiTriggerMode.KSJ_TRIGGERMODE m_TriggerMode;
        KSJApiTriggerMode.KSJ_TRIGGERMETHOD m_TriggerMethod;
        float m_fFixedFrameRate;

        private void UpdateTrigger()//更新触发模式
        {
            string[] szMode = new string[4] { "Internal", "External", "Software", "Fixed Frame Rate" };
            int nMin = 0;
            int nMax = 0;
            KSJApiBase.KSJ_GetParamRange(m_nDeviceCurSel, KSJApiBase.KSJ_PARAM.KSJRIGGER_MODE, ref nMin, ref nMax);
            int i = 0;
            for (i = 0; i < 4; i++)
            {
                comboBox_TriggerMode.Items.Insert(i, szMode[i]);
            }

            if (nMax == 4) comboBox_TriggerMode.Items.Insert(i, "High Low Fixed Frame Rate");

            KSJApiTriggerMode.KSJ_TriggerModeGet(m_nDeviceCurSel, ref m_TriggerMode);
            comboBox_TriggerMode.SelectedIndex = (int)m_TriggerMode;

            string[] szMethod = new string[4] { "Falling Edge", "Rising Edge", "High Level", "Low Level" };
            for (i = 0; i < 4; i++)
            {
                comboBox_TriggerMethod.Items.Insert(i, szMethod[i]);
            }

            KSJApiTriggerMode.KSJ_TriggerMethodGet(m_nDeviceCurSel, ref m_TriggerMethod);
            comboBox_TriggerMethod.SelectedIndex = (int)m_TriggerMethod;

            KSJApiTriggerMode.KSJ_GetFixedFrameRateEx(m_nDeviceCurSel, ref m_fFixedFrameRate);
            FrameRate.Value = (decimal)m_fFixedFrameRate;
        }


        private void comboBox_TriggerMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_TriggerMode = (KSJApiTriggerMode.KSJ_TRIGGERMODE)comboBox_TriggerMode.SelectedIndex;
            KSJApiTriggerMode.KSJ_TriggerModeSet(m_nDeviceCurSel, m_TriggerMode);
        }

        private void comboBox_TriggerMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_TriggerMethod = (KSJApiTriggerMode.KSJ_TRIGGERMETHOD)comboBox_TriggerMethod.SelectedIndex;
            KSJApiTriggerMode.KSJ_TriggerMethodSet(m_nDeviceCurSel, m_TriggerMethod);
        }

        private void FrameRate_ValueChanged(object sender, EventArgs e)
        {
            m_fFixedFrameRate = (float)FrameRate.Value;
            KSJApiTriggerMode.KSJ_SetFixedFrameRateEx(m_nDeviceCurSel, m_fFixedFrameRate);
            
        }
    }
}
