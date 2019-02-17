using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalanceLog
{
    public class WriteLog
    {

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="logInfo"></param>
        public static void WriteBalanceLog(string logInfo)
        {
            WriteLogByDir(logInfo, string.Empty);
        }

        public  decimal WriteBalanceLog(PayInput payInput)
        {
            return ChangeBalance(payInput);
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="logInfo"></param>
        public static void WriteLogByDir(string logInfo, string dirName)
        {
            if (string.IsNullOrEmpty(dirName))
                dirName = "log/";
            else if (!dirName.Contains("/"))
                dirName += "/";
            else
                dirName = "log/";

            string sMonth = DateTime.Now.ToString("MM");
            string sYear = DateTime.Now.ToString("yyyy");
            string sDay = DateTime.Now.ToString("dd");

            string sFileName = AppDomain.CurrentDomain.BaseDirectory + dirName + sYear + "/" + sMonth + "/" + sDay + ".log";

            if (!System.IO.Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + dirName))
            {
                System.IO.Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + dirName);
            }
            if (!System.IO.Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + dirName + sYear))
            {
                System.IO.Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + dirName + sYear);
            }
            if (!System.IO.Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + dirName + sYear + "/" + sMonth))
            {
                System.IO.Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + dirName + sYear + "/" + sMonth);
            }
            if (!System.IO.File.Exists(sFileName))
            {
                System.IO.File.Create(sFileName).Close();
            }

            //写日志
            System.IO.StreamWriter sw = new System.IO.StreamWriter(sFileName, true);
            sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "        "
                + logInfo);
            sw.Close();
        }

        /// <summary>
        /// 更改金额
        /// </summary>
        /// <param name="payInput"></param>
        public static decimal ChangeBalance(PayInput payInput)
        {
            string sPayType = payInput.sPayType;
            string sPayName = string.Empty;
            int iMoney = payInput.iMoney;
            string sUseName = payInput.sUseName;

            //当前余额
            string sXmlResult = string.Empty;
            decimal dNowMoney = 0m;
            decimal dLeftMoney = 0m;
            int iNowMoney = 0;
            sXmlResult = CConfigBalance.GetValue("NoOneMoney.xml", "Money", "PayDir", "", "", "").Trim();
            if (!string.IsNullOrEmpty(sXmlResult))
            {
                dNowMoney = Convert.ToDecimal(sXmlResult);
            }

            iNowMoney = (int)(dNowMoney * 100);
            if (sPayType.Equals("1"))
            {
                sPayName = "存入";
                iNowMoney  += iMoney;
                dLeftMoney = Convert.ToDecimal(iNowMoney) / 100;
            }
            else if(sPayType.Equals("2"))
            {
                sPayName = "支出";
                iNowMoney -= iMoney;
                dLeftMoney = Convert.ToDecimal(iNowMoney) / 100;
            }

            if (!string.IsNullOrEmpty(sPayName))
            {
                CConfigBalance.SetValue("NoOneMoney.xml", "Money", "PayDir", dLeftMoney.ToString(), "", "", "", "", "", "");
                string logInfo = "姓名：" + sUseName + "；支付类型：" + sPayName + "；金额："+ (payInput.iMoney / 100).ToString() +"；剩余金额："+ dLeftMoney.ToString() +"";
                WriteBalanceLog(logInfo);
            }
            return dLeftMoney;
        }
    }
}
