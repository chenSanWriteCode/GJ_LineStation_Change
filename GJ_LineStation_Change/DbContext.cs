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
            //所有用到开发区站点的线路、线路上下行
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
            excuteSql(getCommonUpdateSql("gj_大站表", model)+ " and t.日期 >= '2019-05-01'"); 
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
        /*
        update gj_调度模板车次表 t set t.起始站点id={} where t.线路上下行id={} and t.起始站点id={};
update gj_调度模板车次表 t set t.结束站点id={} where t.线路上下行id={} and t.结束站点id={};
update gj_公交线路表 t set t.主站id={} where t.f_id={} and t.主站id={};
update gj_公交线路表 t set t.副站id={} where t.f_id={} and t.副站id={};
         */
         public void updateBaseInfo(LineStation model)
        {
            excuteSql($"update gj_调度模板车次表 t set t.起始站点id={model.newStationId} where t.线路上下行id={model.UDID} and t.起始站点id={model.stationId}");
            excuteSql($"update gj_调度模板车次表 t set t.结束站点id={model.newStationId} where t.线路上下行id={model.UDID} and t.结束站点id={model.stationId}");

            excuteSql($"update gj_公交线路表 t set t.主站id={model.newStationId} where t.f_id={model.lineId} and t.主站id={model.stationId}");
            excuteSql($"update gj_公交线路表 t set t.副站id={model.newStationId} where t.f_id={model.lineId} and t.副站id={model.stationId}");
        }

        public void updateRec(LineStation model)
        {
            excuteSql($" update gj_gps_短消息历史表 t set t.stopid={model.newStationId} where t.datetime>=to_date('2019-05-01','yyyy-MM-dd') and t.lindid={model.lineId} and t.stopid={model.stationId}");

            excuteSql($" update gj_报警历史表 t set t.站点id={model.newStationId} where t.报警时间>=to_date('2019-05-01','yyyy-MM-dd') and t.线路id={model.lineId} and t.站点id={model.stationId}");

            excuteSql($" update gj_报警历史表票款 t set t.站点id={model.newStationId} where t.报警时间>=to_date('2019-05-01','yyyy-MM-dd') and t.线路id={model.lineId} and t.站点id={model.stationId}");

            excuteSql($" update gj_串车表 t set t.stopId={model.newStationId} where t.gpstime>=to_date('2019-05-01','yyyy-MM-dd') and t.线路id={model.lineId} and t.stopId={model.stationId}");

            excuteSql($" update gj_调度计划表_new t set t.起始站id={model.newStationId} where t.日期>='2019-05-01' and t.线路上下行id={model.UDID} and t.起始站id={model.stationId}");
            excuteSql($" update gj_调度计划表_new t set t.结束站id={model.newStationId} where t.日期>='2019-05-01' and t.线路上下行id={model.UDID} and t.结束站id={model.stationId}");

            excuteSql($"update gj_调度运行表 t set t.start_stopid={model.newStationId} where t.run_day>='2019-05-01' and t.lineid={model.lineId} and t.start_stopid={model.stationId}");
            excuteSql($"update gj_调度运行表 t set t.end_stopid={model.newStationId} where t.run_day>='2019-05-01' and t.lineid={model.lineId} and t.end_stopid={model.stationId}");

            excuteSql($"update gj_调度作业表_new t set t.起始站id={model.newStationId} where t.日期>='2019-05-01' and t.线路id={model.lineId} and t.起始站id={model.stationId}");
            excuteSql($"update gj_调度作业表_new t set t.结束站id={model.newStationId} where t.日期>='2019-05-01' and t.线路id={model.lineId} and t.结束站id={model.stationId}");

            excuteSql($"update gj_中间串车表 t set t.stopid={model.newStationId} where t.gpstime>=to_date('2019-05-01','yyyy-MM-dd') and t.lineid={model.lineId} and t.stopid={model.stationId}");
            excuteSql($"update gj_中间串车表 t set t.stopid2={model.newStationId} where t.gpstime>=to_date('2019-05-01','yyyy-MM-dd') and t.lineid={model.lineId} and t.stopid2={model.stationId}");

            excuteSql($"update gj_趟次 t set t.起始站id={model.newStationId} where t.日期>='2019-05-01' and t.线路id={model.lineId} and t.起始站id={model.stationId}");
            excuteSql($"update gj_趟次 t set t.结束站id={model.newStationId} where t.日期>='2019-05-01' and t.线路id={model.lineId} and t.结束站id={model.stationId}");
        }
    }
    /*
     update gj_gps_短消息历史表 t set t.stopid={} where t.datetime>=to_date('2019-05-01','yyyy-MM-dd') and t.lindid={} and t.stopid={};
update gj_报警历史表 t set t.站点id={} where t.报警时间>='2019-05-01' and t.线路id={} and t.站点id={};
update gj_报警历史表票款 t set t.站点id={} where t.报警时间>='2019-05-01' and t.线路id={} and t.站点id={};
update gj_串车表 t set t.stopId={} where t.gpstime>=to_date('2019-05-01','yyyy-MM-dd') and t.线路id={} and t.stopId={};
update gj_调度计划表_new t set t.起始站id={} where t.日期>='2019-05-01' and t.线路上下行id={} and t.起始站id={};
update gj_调度计划表_new t set t.结束站id={} where t.日期>='2019-05-01' and t.线路上下行id={} and t.结束站id={};

update gj_调度运行表 t set t.start_stopid={} where t.run_day>='2019-05-01' and t.lineid={} and t.start_stopid={};
update gj_调度运行表 t set t.end_stopid={} where t.run_day>='2019-05-01' and t.lineid={} and t.end_stopid={};
update gj_调度作业表_new t set t.起始站id={} where t.日期>='2019-05-01' and t.线路id={} and t.起始站id={};
update gj_调度作业表_new t set t.结束站id={} where t.日期>='2019-05-01' and t.线路id={} and t.结束站id={};

update gj_中间串车表 t set t.stopid={} where t.gpstime>=to_date('2019-05-01','yyyy-MM-dd') and t.lineid={} and t.stopid={};
update gj_中间串车表 t set t.stopid2={} where t.gpstime>=to_date('2019-05-01','yyyy-MM-dd') and t.lineid={} and t.stopid2={};
update gj_趟次 t set t.起始站id={} where t.日期>='2019-05-01' and t.线路id={} and t.起始站id={};
update gj_趟次 t set t.结束站id={} where t.日期>='2019-05-01' and t.线路id={} and t.结束站id={};
         
         */
    public class StationContext : DbContext<Station>
    {
        public List<Station> getStationList(int attach)
        {
            //按照attach排序，这些开发区站点存在 市公交站点的集合（将开发区的站点update成现在查出来的这些站点）
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
        private ILog log = LogManager.GetLogger($"DbContext:{typeof(T).Name}");
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
