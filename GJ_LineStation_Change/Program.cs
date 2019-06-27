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

            Console.WriteLine("enter press can test:");
            Console.ReadLine();
            change.changeYBStation();

            Console.WriteLine("enter press can start 危险点表:");
            Console.ReadLine();
            //change.changeDangerStation();
            Console.ReadKey();

            Console.WriteLine("enter press can start 大站表:");
            Console.ReadLine();
            //change.changeBigStation();
            Console.ReadKey();

            Console.WriteLine("enter press can start 公交车大站表:");
            Console.ReadLine();
            //change.changeBusBigStation();
            Console.ReadKey();


            Console.WriteLine("enter press can start 油补拐点表:");
            Console.ReadLine();
            //change.changeYBStation();
            Console.ReadKey();

            Console.WriteLine("enter press can start gj_线路站点表:");
            Console.ReadLine();
            //change.changeUDStation();
            Console.WriteLine("全部修改完毕");
            Console.ReadKey();
        }
    }


   
}
