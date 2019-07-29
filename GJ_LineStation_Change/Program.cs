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
            Console.WriteLine("开始请按enter:");
            Console.ReadLine();
            change.getLineStationList();
            //Console.WriteLine("enter press can start station:");
            //Console.ReadLine();
            change.getStationList();

            //1 gj_站点表
            Console.WriteLine($"接下来要进行删除站点，合计站点共{change.getAllStationCount()},需要删除的站点共{change.getDeleteStationCount()},是否确认删除站点：y/n");
            doChange(change.deleteStations);
            Console.WriteLine("站点删除完毕");
            //2 gj_调度模板车次表
            Console.WriteLine("接下来 gj_调度模板车次表:y/n");
            doChange(change.updateBaseInfo_MoBanCheCi);
            //3 gj_调度作业表_new
            Console.WriteLine("接下来 gj_调度作业表_new:y/n");
            doChange(change.updateRec_ZuoYe);
            //4 gj_调度计划表_new
            Console.WriteLine("接下来 gj_调度计划表_new:y/n");
            doChange(change.updateRec_JiHua);
            //5 gj_公交线路表
            Console.WriteLine("接下来 gj_公交线路表:y/n");
            doChange(change.updateBaseInfo_XianLu);
            //6 线路站点表
            Console.WriteLine("接下来 gj_线路站点表:y/n");
            doChange(change.changeUDStation);
            //7 gj_趟次
            Console.WriteLine("接下来 gj_趟次:y/n");
            doChange(change.updateRec_TangCi);

            Console.WriteLine("接下来 危险点表:y/n");
            doChange(change.changeDangerStation);

            Console.WriteLine("接下来 大站表:y/n");
            doChange(change.changeBigStation);

            
            Console.WriteLine("接下来 公交车大站表:y/n");
            doChange(change.changeBusBigStation);

            Console.WriteLine("接下来 gj_gps_短消息历史表:y/n");
            doChange(change.updateRec_DuanXiaoXi);
     
            Console.WriteLine("接下来 gj_调度运行表:y/n");
            doChange(change.updateRec_YunXing);

            //Console.WriteLine("接下来 gj_趟次:y/n");
            //doChange(change.updateRec_TangCi);

            Console.WriteLine("接下来 gj_串车表:y/n");
            doChange(change.updateRec_ChuanChe);

            Console.WriteLine("接下来 gj_中间串车表:y/n");
            doChange(change.updateRec_ZhongJianChuanChe);


            Console.WriteLine("接下来 gj_掉头表:y/n");
            doChange(change.updateRec_DiaoTou);

            Console.WriteLine("接下来 gj_调度作业表_check:y/n");
            doChange(change.updateRec_ZuoYe_Check);

            Console.WriteLine("接下来 gj_公交车大站表_new:y/n");
            doChange(change.updateRec_BusDaZhan_New);


            Console.WriteLine("接下来 油补拐点表:y/n");
            doChange(change.changeYBStation);

            Console.WriteLine("接下来 gj_报警历史表票款:y/n");
            doChange(change.updateRec_BaojingHis);

            Console.WriteLine("接下来 gj_报警历史表:y/n");
            doChange(change.updateRec_Baojing);

            Console.WriteLine("全部修改完毕，如果还要市公交或开发区没有处理，请重启程序");
            
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
