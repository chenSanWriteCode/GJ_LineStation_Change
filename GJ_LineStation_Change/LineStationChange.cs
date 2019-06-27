using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using SqlSugar;

namespace GJ_LineStation_Change
{
    public class LineStationChange
    {
        private ILog log = LogManager.GetLogger("LineStationChange");
        private List<Station> stationList;
        private List<LineStation> lineStationList;
        private int Attachtype;

        public LineStationChange(int type)
        {
            this.Attachtype = type;
        }
        public void getLineStationList()
        {
            LineStationContext context = new LineStationContext();
            lineStationList = context.getLineStationList(Attachtype);
        }

        public void getStationList()
        {
            StationContext context = new StationContext();
            stationList = context.getStationList(Attachtype);
            lineStationList.ForEach(x =>
            {
                var model = stationList.FirstOrDefault(s => s.stationName == x.stationName && s.derection == x.derection);
                if (model!=null)
                {
                    x.attach = 1;
                    x.newStationId = model.stationId;
                }
            });
            log.Info($"站点个数共{stationList.Count}");
            if (Attachtype==1)
            {
                log.Info($"需要修改的上下行共{lineStationList.Where(x => x.attach == 1).Count()}条");
                lineStationList.Where(x => x.attach == 1).OrderBy(x=> x.lineName).ToList().ForEach(x => log.Info($"{x.lineName},{x.stationName},{x.derection}"));
            }
            else
            {
                log.Info($"需要修改的线路共{lineStationList.Where(x => x.attach == 1).GroupBy(x => x.lineName).Count()}条");
                lineStationList.Where(x => x.attach == 1).GroupBy(x => x.lineName).ToList().ForEach(x=>log.Info(x.Key));
            }
            

        }

        public void changeUDStation()
        {
            LineStationContext context = new LineStationContext();
            lineStationList.Where(x=>x.attach==1).ToList().ForEach(x => context.updateLineStation(x));
        }
        public void changeDangerStation()
        {
            LineStationContext context = new LineStationContext();
            lineStationList.Where(x => x.attach == 1).ToList().ForEach(x => context.updateLineDanger(x));
        }
        public void changeBigStation()
        {
            LineStationContext context = new LineStationContext();
            lineStationList.Where(x => x.attach == 1).ToList().ForEach(x => context.updateLineBigStation(x));
        }
        public void changeBusBigStation()
        {
            LineStationContext context = new LineStationContext();
            lineStationList.Where(x => x.attach == 1).ToList().ForEach(x => context.updateLineBigStationOfBus(x));
        }
        public void changeYBStation()
        {
            LineStationContext context = new LineStationContext();
            var group = lineStationList.Where(x => x.attach == 1).GroupBy(x => x.UDID);
            foreach (var item in group)
            {
                //context.backYBGuaiDian(item.Key, Attachtype);
                //context.updateYBOtherInfo(item.Key, Attachtype);
            }
            //根据上下行id与站点id 更新新的站点id
            //lineStationList.Where(x => x.attach == 1).ToList().ForEach(x => context.updateYBStationId(x,Attachtype));
        }


    }
    public class LineStation
    {
        public decimal lineId { get; set; }
        public string lineName { get; set; }
        public decimal UDID { get; set; }

        public decimal stationId { get; set; }
        public string stationName { get; set; }
        public string derection { get; set; }
        public decimal newStationId { get; set; }
        public int attach { get; set; } = 2;
    }
    public class Station
    {
        public decimal stationId { get; set; }
        public string stationName { get; set; }
        public string derection { get; set; }
    }
}
