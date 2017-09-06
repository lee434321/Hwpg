using System;
using System.Collections.Generic;
using System.Web;
using System.Data;

/// <summary>
/// 接口 3.1 用户登录校验处理类
/// </summary>
public class Biz31 : BizFactory
{
    Req31 req = null;
    
    public Biz31()
    {
       
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="postStr"></param>
    public Biz31(string postStr)
    {
        this.BIZ_NAME = "接口3.1";
        this.req = Common.Deserialize<Req31>(postStr) as Req31;
    }

    /// <summary>
    /// 请求处理主函数
    /// </summary>
    /// <returns></returns>
    public override string InvokeRequest()
    {
        Resp31 resp = new Resp31();
        DataTable dtUser= this.DAL_Select(); //取登录用户信息

        if (dtUser.Rows.Count > 0)
        {
            string pwd = Common.DbNull2Str(dtUser.Rows[0]["PASSWORD"]);
            if (this.req.data.password == pwd)
            {
                resp.data.result = "100";
                resp.data.message = "验证成功";
                Common.CurrentUser = this.req.data.username;
            }
            else
            {
                resp.data.result = "200";
                resp.data.message = "密码错误";
            }
        }
        else {
            resp.data.result = "200";
            resp.data.message = "用户名不存在";
        }

        return  Common.Serialize<Resp31>(resp);
    }

    private DataTable DAL_Select()
    {
        DbHelper db = new DbHelper(Common.OracleConnStrLocal, false);
        try
        {
            string sql = "select * from SYS_USERS t where 1=1 ";
            sql += " and loginname='" + this.req.data.username + "'";

            DataTable dt = db.ExecuteDT(sql);
            db.Commit();
            return dt;
        }
        catch (Exception err)
        {
            db.Abort();
            throw err;
        }
    }
}