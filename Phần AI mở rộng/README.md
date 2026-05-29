# Phan AI mo rong

Phan nay bo sung phan quyen dang nhap cho ung dung quan ly khach san.

- `Admin`: truy cap tat ca chuc nang, duoc them/sua/xoa du lieu.
- `Manager`: xem dashboard, them/sua phong, xem va xu ly khach hang, check-out. Khong duoc xoa phong va khong quan ly nhan vien.
- `Staff`: dang ky khach, xem chi tiet khach va check-out. Khong duoc quan ly phong va nhan vien.

Code chinh nam trong `RolePermissionService.cs`. Sau khi login, `MainWindow` nhan user hien tai va an cac menu ma role khong duoc phep dung.

Tinh nang online/offline:

- Khi user login, `UserPresenceService` danh dau `IsOnline = true`.
- Khi user logout hoac dong cua so chinh, he thong danh dau `IsOnline = false` va cap nhat `LastSeenAt`.
- `Admin` xem duoc trang thai `Manager` va `Staff`.
- `Manager` xem duoc trang thai `Staff`.
- `Staff` khong thay menu `User Status`.
