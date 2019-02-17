using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalanceLog
{
    class Program
    {
        static void Main(string[] args)
        {
            bool exec = true;
            do
            {
                string sPayType = string.Empty;
                string sUseName = string.Empty;
                string sMoney = string.Empty;
                //单位：分
                int iMoney = 0;
                //余额
                decimal dLeftMoney = 0m;

                Console.WriteLine("欢迎使用即时记账！！");
                Console.Write("存入：1；支出：2：");
                sPayType = Console.ReadLine();
                Console.Write("当前使用人：");
                sUseName = Console.ReadLine();
                Console.Write("金额：");
                sMoney = Console.ReadLine();
                iMoney = (int)(Convert.ToDecimal(sMoney) * 100);
                WriteLog writeLog = new WriteLog();

                if (iMoney <= 0)
                {
                    Console.Write("你在逗我玩？？？");
                }
                else
                {
                    PayInput payInput = new PayInput()
                    {
                        sPayType = sPayType,
                        sUseName = sUseName,
                        iMoney = iMoney,
                    };
                    dLeftMoney = writeLog.WriteBalanceLog(payInput);
                }
                Console.WriteLine("当前余额：" + dLeftMoney.ToString());

                Console.WriteLine("是否要继续记账 1：是；2：不是");
                string sContinue = Console.ReadLine();
                exec = sContinue.Equals("1") ? true : false;
            }
            while (exec);
        }
    }
}
