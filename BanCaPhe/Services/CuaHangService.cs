using BanCaPhe.Models;
using Dapper;
using System.Data;
using System.Linq;

namespace BanCaPhe.Services
{
    public class CuaHangService
    {
        public ThongTinCuaHang GetThongTin()
        {
            return Models.StoreHelper.QueryFirstOrDefault<ThongTinCuaHang>("sp_GetThongTinCuaHang");
        }
    }
}
