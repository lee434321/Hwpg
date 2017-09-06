using System;
using System.Collections.Generic;
using System.Web;
using System.Net;
using System.IO;

/// <summary>
/// 3.7	获取对账单
/// act=getstatement
/// </summary>
public class Biz37 : BizFactory
{
    Req37 req = null;

    public Biz37()
    {
    }

    /// <summary>
    /// 构造函数，反序列化转成参数实例
    /// </summary>
    /// <param name="postStr"></param>
    public Biz37(string postStr)
    {
        this.BIZ_NAME = "接口3.7";
        this.req = Common.Deserialize<Req37>(postStr) as Req37;
    }

    /// <summary>
    /// 发起获取账单请求
    /// </summary>
    /// <returns></returns>
    public override string InvokeRequest()
    {
        Resp37 resp = new Resp37();
        resp.code = "100";
        resp.msg = "接口操作成功!";
        resp.status = "true";
        resp.time = Common.Today;

        DateTime start;

        try
        {
            if (Common.CurrentUser == "")
                throw new Exception("登录用户为空");

            string url = Common.RemoteUrl + "PNReport/monthlystatement.ashx?";//PropertyOne中一般处理程序MonthlyStatement的访问地址
            url += "lease_number=" + this.req.data.leasenum;
            url += "&start_date=" + this.req.data.startdate;
            url += "&end_date=" + this.req.data.enddate;
            url += "&statementnum=" + this.req.data.statementnum;
            url += "&requestby=" + Common.CurrentUser;//附加查询字符串

            start = DateTime.Now;
            //创建web请求对象
            HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(url);
            webReq.Method = "GET";
            webReq.ContentType = "application/json";
            webReq.PreAuthenticate = true;
            ICredentials credential = new NetworkCredential(Common.Credentials.name, Common.Credentials.pwd, Common.Credentials.domain);
            webReq.Credentials = credential;

            //获取返回信息
            HttpWebResponse webResp = webReq.GetResponse() as HttpWebResponse;
            Stream s = webResp.GetResponseStream();
            StreamReader sr = new StreamReader(s, System.Text.Encoding.UTF8);
            string respData = "";
            string jsonResp = "";
            while ((respData = sr.ReadLine()) != null)
            {
                jsonResp += respData;
            }

            //返回信息转化为实体类
            Resp37Ex respEx = (Resp37Ex)Common.Deserialize<Resp37Ex>(jsonResp);
            Common.WriteLog("POne返回:" + jsonResp);
            /// 2.获取远程文件
            if (this.req.data.statementnum != "") //如果statementseq不为空，则需要获取pdf到本地
            {
                Common.WriteLog("准备下载PDF");
                WebClient client = new WebClient(); //创建webclient实例,用于最终的下载
                client.Credentials = credential;    //网络访问信任权限配置(web.config中确保key=credentials 已经正确配置)
                client.Headers.Add(HttpRequestHeader.Accept, @"text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
                client.Headers.Add(HttpRequestHeader.ContentType, "application/octet-stream");
                client.Headers.Add(HttpRequestHeader.UserAgent, "anything");
                for (int i = 0; i < respEx.result.Count; i++)
                {
                    url = Common.RemoteUrl + "pnreport/" + respEx.result[i].filename;       //生成下载文件url
                    Common.WriteLog("下载文件路径:" + url + "|" + Common.StatementPath + Path.GetFileName(url));
                    client.DownloadFile(url, Common.StatementPath + Path.GetFileName(url)); //远程pdf文件被下载到本地指定目录
                }
            }

            double seconds = DateTime.Now.Subtract(start).TotalSeconds;
            /// 3.输出结果
            resp.data.result = "100";
            resp.data.message = "获取账单成功" + "(耗时" + seconds.ToString() + "秒)";
            for (int i = 0; i < respEx.result.Count; i++)
            {
                Resp37_2 si = new Resp37_2();
                si.paymentduedate = respEx.result[i].paymentduedate;
                si.statementamount = respEx.result[i].statementamount;
                si.statementdate = respEx.result[i].statementdate;
                si.statementnum = respEx.result[i].statementnum;
                si.url = respEx.result[i].filename;
                resp.data.statementinfo.Add(si);
            }
            resp.data.statementnum = respEx.result.Count;
        }
        catch (WebException webErr)
        {
            Common.WriteLog("catch web exception" + webErr.Message);
        }
        catch (Exception err)
        {
            resp.code = "200";
            resp.msg = "接口操作失败!";
            resp.status = "false";
            resp.time = Common.Today;
            resp.data.result = "200";
            resp.data.message = err.Message;
        }

        return Common.Serialize<Resp37>(resp);
    }
}

/* 参考
 * 
    //Set credentials of the current security context
    request.PreAuthenticate = true;
    request.UseDefaultCredentials = true;
    ICredentials credentials = new NetworkCredential( "Username", "password", "Domain"); //I used my username and password here
    request.Credentials = credentials;
    //request.Credentials = CredentialCache.DefaultCredentials;
    request.Method = "PUT";

    // Create buffer to transfer file
    byte[] fileBuffer = new byte[1024];

    // Write the contents of the local file to the request stream.
    using (Stream stream = request.GetRequestStream())
    {
        //Load the content from local file to stream
        using (FileStream fsWorkbook = File.Open(sourceFilePath, FileMode.Open, FileAccess.Read))
        {
            //Get the start point
            int startBuffer = fsWorkbook.Read(fileBuffer, 0, fileBuffer.Length);
            for (int i = startBuffer; i > 0; i = fsWorkbook.Read(fileBuffer, 0, fileBuffer.Length))
            {
                stream.Write(fileBuffer, 0, i);
            }

        }
    }

 */