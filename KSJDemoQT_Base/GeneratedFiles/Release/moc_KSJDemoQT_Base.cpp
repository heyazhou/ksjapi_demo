/****************************************************************************
** Meta object code from reading C++ file 'KSJDemoQT_Base.h'
**
** Created by: The Qt Meta Object Compiler version 67 (Qt 5.7.0)
**
** WARNING! All changes made in this file will be lost!
*****************************************************************************/

#include "../../KSJDemoQT_Base.h"
#include <QtCore/qbytearray.h>
#include <QtCore/qmetatype.h>
#if !defined(Q_MOC_OUTPUT_REVISION)
#error "The header file 'KSJDemoQT_Base.h' doesn't include <QObject>."
#elif Q_MOC_OUTPUT_REVISION != 67
#error "This file was generated using the moc from 5.7.0. It"
#error "cannot be used with the include files from this version of Qt."
#error "(The moc has changed too much.)"
#endif

QT_BEGIN_MOC_NAMESPACE
struct qt_meta_stringdata_KSJDemoQT_Base_t {
    QByteArrayData data[12];
    char stringdata0[284];
};
#define QT_MOC_LITERAL(idx, ofs, len) \
    Q_STATIC_BYTE_ARRAY_DATA_HEADER_INITIALIZER_WITH_OFFSET(len, \
    qptrdiff(offsetof(qt_meta_stringdata_KSJDemoQT_Base_t, stringdata0) + ofs \
        - idx * sizeof(QByteArrayData)) \
    )
static const qt_meta_stringdata_KSJDemoQT_Base_t qt_meta_stringdata_KSJDemoQT_Base = {
    {
QT_MOC_LITERAL(0, 0, 14), // "KSJDemoQT_Base"
QT_MOC_LITERAL(1, 15, 43), // "on_DEVICE_LIST_ComboBox_curre..."
QT_MOC_LITERAL(2, 59, 0), // ""
QT_MOC_LITERAL(3, 60, 5), // "index"
QT_MOC_LITERAL(4, 66, 35), // "on_ExposureTimeSpinBox_valueC..."
QT_MOC_LITERAL(5, 102, 4), // "arg1"
QT_MOC_LITERAL(6, 107, 36), // "on_ExposureLinesSpinBox_value..."
QT_MOC_LITERAL(7, 144, 27), // "on_GainSpinBox_valueChanged"
QT_MOC_LITERAL(8, 172, 30), // "on_SetPreviewFovButton_clicked"
QT_MOC_LITERAL(9, 203, 24), // "on_PreviewButton_clicked"
QT_MOC_LITERAL(10, 228, 30), // "on_SetCaptureFovButton_clicked"
QT_MOC_LITERAL(11, 259, 24) // "on_CaptureButton_clicked"

    },
    "KSJDemoQT_Base\0"
    "on_DEVICE_LIST_ComboBox_currentIndexChanged\0"
    "\0index\0on_ExposureTimeSpinBox_valueChanged\0"
    "arg1\0on_ExposureLinesSpinBox_valueChanged\0"
    "on_GainSpinBox_valueChanged\0"
    "on_SetPreviewFovButton_clicked\0"
    "on_PreviewButton_clicked\0"
    "on_SetCaptureFovButton_clicked\0"
    "on_CaptureButton_clicked"
};
#undef QT_MOC_LITERAL

static const uint qt_meta_data_KSJDemoQT_Base[] = {

 // content:
       7,       // revision
       0,       // classname
       0,    0, // classinfo
       8,   14, // methods
       0,    0, // properties
       0,    0, // enums/sets
       0,    0, // constructors
       0,       // flags
       0,       // signalCount

 // slots: name, argc, parameters, tag, flags
       1,    1,   54,    2, 0x08 /* Private */,
       4,    1,   57,    2, 0x08 /* Private */,
       6,    1,   60,    2, 0x08 /* Private */,
       7,    1,   63,    2, 0x08 /* Private */,
       8,    0,   66,    2, 0x08 /* Private */,
       9,    0,   67,    2, 0x08 /* Private */,
      10,    0,   68,    2, 0x08 /* Private */,
      11,    0,   69,    2, 0x08 /* Private */,

 // slots: parameters
    QMetaType::Void, QMetaType::Int,    3,
    QMetaType::Void, QMetaType::Int,    5,
    QMetaType::Void, QMetaType::Int,    5,
    QMetaType::Void, QMetaType::Int,    5,
    QMetaType::Void,
    QMetaType::Void,
    QMetaType::Void,
    QMetaType::Void,

       0        // eod
};

void KSJDemoQT_Base::qt_static_metacall(QObject *_o, QMetaObject::Call _c, int _id, void **_a)
{
    if (_c == QMetaObject::InvokeMetaMethod) {
        KSJDemoQT_Base *_t = static_cast<KSJDemoQT_Base *>(_o);
        Q_UNUSED(_t)
        switch (_id) {
        case 0: _t->on_DEVICE_LIST_ComboBox_currentIndexChanged((*reinterpret_cast< int(*)>(_a[1]))); break;
        case 1: _t->on_ExposureTimeSpinBox_valueChanged((*reinterpret_cast< int(*)>(_a[1]))); break;
        case 2: _t->on_ExposureLinesSpinBox_valueChanged((*reinterpret_cast< int(*)>(_a[1]))); break;
        case 3: _t->on_GainSpinBox_valueChanged((*reinterpret_cast< int(*)>(_a[1]))); break;
        case 4: _t->on_SetPreviewFovButton_clicked(); break;
        case 5: _t->on_PreviewButton_clicked(); break;
        case 6: _t->on_SetCaptureFovButton_clicked(); break;
        case 7: _t->on_CaptureButton_clicked(); break;
        default: ;
        }
    }
}

const QMetaObject KSJDemoQT_Base::staticMetaObject = {
    { &QDialog::staticMetaObject, qt_meta_stringdata_KSJDemoQT_Base.data,
      qt_meta_data_KSJDemoQT_Base,  qt_static_metacall, Q_NULLPTR, Q_NULLPTR}
};


const QMetaObject *KSJDemoQT_Base::metaObject() const
{
    return QObject::d_ptr->metaObject ? QObject::d_ptr->dynamicMetaObject() : &staticMetaObject;
}

void *KSJDemoQT_Base::qt_metacast(const char *_clname)
{
    if (!_clname) return Q_NULLPTR;
    if (!strcmp(_clname, qt_meta_stringdata_KSJDemoQT_Base.stringdata0))
        return static_cast<void*>(const_cast< KSJDemoQT_Base*>(this));
    return QDialog::qt_metacast(_clname);
}

int KSJDemoQT_Base::qt_metacall(QMetaObject::Call _c, int _id, void **_a)
{
    _id = QDialog::qt_metacall(_c, _id, _a);
    if (_id < 0)
        return _id;
    if (_c == QMetaObject::InvokeMetaMethod) {
        if (_id < 8)
            qt_static_metacall(this, _c, _id, _a);
        _id -= 8;
    } else if (_c == QMetaObject::RegisterMethodArgumentMetaType) {
        if (_id < 8)
            *reinterpret_cast<int*>(_a[0]) = -1;
        _id -= 8;
    }
    return _id;
}
QT_END_MOC_NAMESPACE
