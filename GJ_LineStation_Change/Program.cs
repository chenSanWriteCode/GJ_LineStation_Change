using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace GJ_LineStation_Change
{
    class Program
    {
        static void Main(string[] args)
        {
            int type=-1;
            bool typeCorrect = false;
            while (!typeCorrect)
            {
                Console.WriteLine("市公交:1 or 开发区：2");
                typeCorrect = int.TryParse(Console.ReadLine(),out type);
            }

            LineStationChange change = new LineStationChange(type);
            Console.WriteLine("enter press can start linestation:");
            Console.ReadLine();
            change.getLineStationList();


            Console.WriteLine("enter press can start station:");
            Console.ReadLine();
            change.getStationList();


            Console.WriteLine("shall we start 危险点表:y/n");
            doChange(change.changeDangerStation);

            Console.WriteLine("shall we start 大站表:y/n");
            doChange(change.changeBigStation);

            
            Console.WriteLine("shall we start 公交车大站表:y/n");
            doChange(change.changeBusBigStation);

            Console.WriteLine("shall we start gj_线路站点表:y/n");
            doChange(change.changeUDStation);

            Console.WriteLine("shall we start gj_调度模板车次表、gj_公交线路表:y/n");
            doChange(change.updateBaseInfo);

            Console.WriteLine("shall we start gj_gps_短消息历史表、gj_报警历史表、gj_报警历史表票款、gj_串车表、gj_调度计划表_new、gj_调度运行表、gj_调度作业表_new、gj_中间串车表、gj_趟次:y/n");
            doChange(change.updateRec);

            Console.WriteLine("shall we start 油补拐点表:y/n");
            doChange(change.changeYBStation);
            Console.WriteLine("全部修改完毕，如果还要市公交或开发区没有处理，请重启程序");
            Console.WriteLine($"接下来要进行删除站点，合计站点共{change.getAllStationCount()},需要删除的站点共{change.getDeleteStationCount()},是否确认删除站点：y/n");
            doChange(change.deleteStations);
            Console.WriteLine("站点删除完毕，请关闭程序");
            Console.ReadKey();
        }
        public static  void doChange(Action method)
        {
            if (Console.ReadLine() == "y")
            {
                method();
            }
        }
    }


   
}
