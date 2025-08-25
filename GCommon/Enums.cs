using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCommon
{
    public enum RequestState
    {
        Failed = -1,
        NotAuth = 0,
        Success = 1,
        message = 2,
    }

    public enum TrangThai
    {
        NHAP = 0,
        CHO_DUYET = 1,
        DA_DUYET = 2,
        TU_CHOI = 3,
        HUY_DUYET = 4,
        KET_THUC = 5,
        KHOA = 6,
        CHOXOA = 98,
        DAXOA = 99,
    }
    public enum TrangThaiMuonTra
    {
        DANG_MUON = 0,
        DA_TRA = 1
    }

    public enum TrangThaiHS
    {
        NHAP = 0,
        THI_HANH = 1,
        HOAN_THANH = 2,
        DAXOA = 99,
    }
    public enum TinhTrangLuuTru
    {
        [Description("Trong kho")]
        TRONG_KHO = 0,
        [Description("Đang cho mượn")]
        DANG_CHO_MUON = 1,
        [Description("Đã trang cấp")]
        DA_TRANG_CAP = 2,
        [Description("Kho mượn")]
        KHO_MUON = 3,
    }
    public enum PhanLoai
    {
        [Description("Thiết bị cho thuê, mượn")]
        ThueMuon = 1,
        [Description("Thiết bị trang cấp")]
        TrangCap = 2,
    }
    public enum LoaiBanGiao
    {
        TienMat = 1,
        ThietBi,
        MatHong,
        ThanhLy
    }
    public enum LoaiDonVi
    {
        COQUAN = 1,
        UBND = 2,
    }

    public static class DsDoiTuong
    {
        public static readonly string HT_NGUOIDUNG = "HT_NGUOIDUNG";
        public static readonly string DM_DANHMUC = "DM_DANHMUC";
        public static readonly string DM_DANHMUC_ITEM = "DM_DANHMUC_ITEM";
        public static readonly string HS_FORM_DATA = "HS_FORM_DATA";
        public static readonly string HS_HOSO = "HS_HOSO";
        public static readonly string HS_HOSO_DT = "HS_HOSO_DT";
        public static readonly string TANGVAT = "TANGVAT";
        public static readonly string GIAYTO = "GIAYTO";
        public static readonly string THONGTIN = "THONGTIN";
    }


    public static class DsChucNang
    {
        public static readonly long DonVi = 1;
        public static readonly long NhomQuyen = 2;
        public static readonly long DiaBan = 20004;
        public static readonly long KhoLuuTru = 70049;
        public static readonly long DonViTinh = 70051;
        public static readonly long NhaCungCap = 70050;
        public static readonly long CanBo = 70045;
        public static readonly long ChucVu = 10015;
    }

    public static class DsDanhMucItem
    {
        
    }


    public enum Quyen
    {
        [Description("Xem danh sách")]
        Xem,
        [Description("Thêm")]
        Them,
        [Description("Sửa")]
        Sua,
        [Description("Xoá")]
        Xoa,
        [Description("Duyệt")]
        Duyet,
        [Description("Huỷ duyệt")]
        HuyDuyet,
        [Description("Đăng nhập")]
        DangNhap,
        [Description("Đăng xuất")]
        DangXuat,
        [Description("Chuyển đổi")]
        ChuyenDoi,
        [Description("Gán đơn vị")]
        GanDonVi,
        [Description("Đặt lại mật khẩu")]
        DatLaiMatKhau,
        [Description("Sửa thông tin")]
        SuaThongTin,
        [Description("Phân quyền")]
        PhanQuyen,
        [Description("Cấu hình")]
        CauHinh,
        [Description("Gán người dùng")]
        GanNguoiDung,
        [Description("Đổi mật khẩu")]
        DoiMatKhau,
        [Description("Phục hồi")]
        PhucHoi,
        [Description("Xoá vĩnh viễn")]
        XoaVinhVien,
        [Description("Cấu hình cột")]
        CauHinhCot,
        [Description("Chờ duyệt")]
        ChoDuyet,
        [Description("Tiếp nhận")]
        TiepNhan,
        [Description("Xử lý")]
        XuLy,
        [Description("Phê duyệt")]
        PheDuyet,
        [Description("Trả lại")]
        TraLai,
        [Description("Trả kết quả")]
        TraKetQua,
        [Description("Tra cứu")]
        TraCuu,
        [Description("Khoá")]
        Khoa,
        [Description("Mở khoá")]
        MoKhoa,
        [Description("Đánh giá")]
        DanhGia,
        [Description("Thẩm định")]
        ThamDinh,
        [Description("Sao lưu")]
        SaoLuu,
        [Description("Quản trị đơn vị")]
        QuanTriDonVi,
        [Description("Quản trị hệ thống")]
        QuanTriHeThong,
    }

    public enum NhomChucNang
    {
        [Description("Quản trị hệ thống")]
        QuanTriHeThong,
        [Description("Danh mục dùng chung")]
        DanhMucDungChung,
        [Description("Báo cáo")]
        BaoCao,
        [Description("Thiết bị")]
        ThietBi
    }

    public enum LoaiTinNhan
    {
        NguoiDung,
        HeThong
    }
    public enum TrangThaiTinNhan
    {
        ChuaXem,
        DaXem
    }

    public enum TrangThaiTep
    {
        KHONG_SU_DUNG,
        DANG_SU_DUNG,
    }

}
