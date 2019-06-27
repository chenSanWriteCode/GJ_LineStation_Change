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

            bool startFlag =false;

            Console.WriteLine("shall we start 危险点表:y/n");
            startFlag = Console.ReadLine()=="y";
            //change.changeDangerStation();
            Console.ReadKey();

            Console.WriteLine("shall we start 大站表:y/n");
            startFlag = Console.ReadLine() == "y";
            //change.changeBigStation();
            Console.ReadKey();

            
            Console.WriteLine("shall we start 公交车大站表:y/n");
            startFlag = Console.ReadLine() == "y";
            //change.changeBusBigStation();
            Console.ReadKey();

            Console.WriteLine("shall we start 油补拐点表:y/n");
            startFlag = Console.ReadLine() == "y";
            //change.changeYBStation();
            Console.ReadKey();

            Console.WriteLine("shall we start gj_线路站点表:y/n");
            startFlag = Console.ReadLine() == "y";
            //change.changeUDStation();
            Console.WriteLine("全部修改完毕");
            Console.ReadKey();
        }
    }


   
}
