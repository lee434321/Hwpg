using System;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// 接口 3.2 用户忘记密码处理类
/// </summary>
public class Biz32:BizFactory
{
    Req32 req = null;
	public Biz32()
	{
	
	}

    public Biz32(string postStr)
    {
        this.BIZ_NAME = "接口3.2";
        this.req = Common.Deserialize<Req32>(postStr) as Req32;
    }

    public override string InvokeRequest()
    {
        Resp32 resp = new Resp32();
        
        try
        {
            /// 1.生成临时密码
            string tmpStr = System.Guid.NewGuid().ToString().Substring(0, 6);
            //DAL_Update_PWD(tmpStr);
            /// 2.发送邮件
            Common.SendEmail(this.req.data.email);
        }
        catch
        { }

        return Common.Serialize<Resp32>(resp);
    }

    private void SendEmail(string emailAddr)
    {
    }

    private void DAL_Update_PWD(string pwd)
    {
        DbHelper db = new DbHelper(Common.OracleConnStrLocal, true);
        try
        {
            string sql = "";
            sql += "update sys_users set password='" + pwd + "' where 1=1 ";
            sql += " and loginname='" + req.data.username + "'";
            db.ExecuteNonQuery(sql);
            db.Commit();
        }
        catch(Exception err )
        {
            db.Abort();
            throw err;
        }
    }
}