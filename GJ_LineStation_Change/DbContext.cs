using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GJ_LineStation_Change;
using log4net;
using SqlSugar;
using Newtonsoft.Json;

namespace GJ_LineStation_Change
{

    public class LineStationContext : DbContext<LineStation>
    {
        public List<LineStation> getLineStationList(int attach)
        {
            string sql = $"select l.f_id lineId,l.线路名称 lineName,ud.f_id UDID,ud.公交线路,ls.站点id stationId,s.名称 stationName,s.地理方向 derection,ls.顺序 from gj_公交线路上下行表 ud,gj_公交线路表 l,gj_线路站点表 ls,gj_站点 s where ls.线路上下行id = ud.f_id and ud.线路id = l.f_id and ls.站点id = s.f_id and l.attach = {attach} and s.attach = 2 order by l.线路名称,ud.公交线路,ls.顺序";
            return base.getListBySql(sql);
        }
        private string getCommonUpdateSql(string tableName, LineStation model)
        {
            return $"update {tableName} t  set t.站点id={model.newStationId} where t.线路上下行id={model.UDID} and t.站点id={model.stationId} ";
        }
        public void updateLineStation(LineStation model)
        {
            excuteSql(getCommonUpdateSql("test_qi", model));
        }

        public void updateLineDanger(LineStation model)
        {
            excuteSql(getCommonUpdateSql("gj_线路危险点表", model));
        }
        public void updateLineBigStationOfBus(LineStation model)
        {
            excuteSql(getCommonUpdateSql("gj_公交车大站表", model));
        }
        public void updateLineBigStation(LineStation model)
        {
            excuteSql(getCommonUpdateSql("gj_大站表", model));
        }
        public void updateYBStationId(LineStation model, int type)
        {
            string tableExtend = type == 1 ? "@youbu" : "@youbu.kfq";
            base.excuteSql(getCommonUpdateSql($"YB_线路拐点表{tableExtend}", model));
            
        }
        public void backYBGuaiDian(decimal UDID, int type)
        {
            string tableExtend = type == 1 ? "@youbu" : "@youbu.kfq";
            string sql = $"insert into YB_线路拐点表back{tableExtend} select * from YB_线路拐点表{tableExtend} t where t.线路上下行id={UDID}";
            base.excuteSql(sql);
        }
        public void updateYBOtherInfo(decimal UDID, int type)
        {
            string tableExtend = type == 1 ? "@youbu" : "@youbu.kfq";
            string sql = $"update YB_线路拐点表@youbu t set t.version=seq_update_version.nextval,t.is_upload=0 where t.线路上下行id={UDID} ";
            base.excuteSql(sql);
        }
    }

    public class StationContext : DbContext<Station>
    {
        public List<Station> getStationList(int attach)
        {
            string sql = $"select sss.f_id stationId,sss.名称 stationName,sss.地理方向 derection,sss.attach from (select s.f_id,s.名称,s.地理方向,s.attach,row_number() over(partition by s.名称,s.地理方向 order by s.attach)sn from gj_站点 s , (select distinct s.名称, s.地理方向 from gj_公交线路上下行表 ud, gj_公交线路表 l, gj_线路站点表 ls, gj_站点 s where ls.线路上下行id = ud.f_id and ud.线路id = l.f_id and ls.站点id = s.f_id and l.attach = {attach} and s.attach = 2)ss where  s.名称 = ss.名称 and s.地理方向 = ss.地理方向 order by s.名称,s.地理方向)sss where sn = 1 and attach=1";
            return getListBySql(sql);
        }

        public int getAllStationCount()
        {
            string sql = "select count(1) from (select s.f_id, s.名称, s.地理方向, s.attach, row_number() over(partition by s.名称, s.地理方向 order by s.attach, s.f_id) sn  from gj_站点 s where s.gpsx2 is not null and s.stationtype=1 and s.attach in (1,2))ss";
            return selectCount(sql);
        }

        public int getDeleteStationCount()
        {
            string sql = "select count(1) from (select s.f_id, s.名称, s.地理方向, s.attach, row_number() over(partition by s.名称, s.地理方向 order by s.attach, s.f_id) sn  from gj_站点 s where s.gpsx2 is not null and s.stationtype=1 and s.attach in (1,2))ss where ss.sn>1";
            return selectCount(sql);
        }

        public int deleteStations()
        {
            string sql = "delete from gj_站点 s where s.f_id in (select f_id from (select s.f_id, s.名称, s.地理方向, s.attach, row_number() over(partition by s.名称, s.地理方向 order by s.attach, s.f_id) sn  from gj_站点 s where s.gpsx2 is not null and s.stationtype=1 and s.attach in (1,2))ss where ss.sn>1)";
            return excuteSql(sql);
        }
    }



    public class DbContext<T> where T : class, new()
    {
        private ILog log = LogManager.GetLogger($"DbContext:{nameof(T)}");
        private static string connStr = ConfigurationManager.ConnectionStrings["Oracle"].ConnectionString;
        public SqlSugarClient DB;
        public SimpleClient<T> CurrentDB { get { return new SimpleClient<T>(DB); } }
        public DbContext()
        {
            DB = new SqlSugarClient(new ConnectionConfig
            {
                ConnectionString = connStr,
                DbType = DbType.Oracle,
                IsAutoCloseConnection = true
            });
        }

        public List<T> getListBySql(string sql)
        {
            List<T> list = new List<T>();
            try
            {
                list = DB.Ado.SqlQuery<T>(sql);
                log.Info(sql);
            }
            catch (Exception err)
            {
                log.Error(err.Message);
            }
            return list;
        }
        public int excuteSql(string sql)
        {
            int count = 0;
            var result = DB.Ado.UseTran(() =>
            {
                count = DB.Ado.ExecuteCommand(sql);
            });
            if (!result.IsSuccess)
            {
                log.Error(result.ErrorMessage + sql);
            }
            return count;
        }

        public int selectCount(string sql)
        {
            int count = 0;
            try
            {
                count = DB.Ado.GetInt(sql);
            }
            catch (Exception err)
            {
                log.Error(err.Message);
            }
            return count;
        }
    }
}
