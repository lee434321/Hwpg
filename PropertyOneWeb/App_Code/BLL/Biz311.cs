using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// 接口3.11获取Notice信息
/// </summary>
public class Biz311 :BizFactory
{
    Req311 req = null;
	public Biz311()
	{
		
	}

    public Biz311(string postStr)
    {
        this.BIZ_NAME = "接口3.11";
        req = Common.Deserialize<Req311>(postStr) as Req311;
    }

    public override string InvokeRequest()
    {
        Resp311 resp = new Resp311();

        try
        {
            Criteria c = new Criteria();
            c.noticeType = this.req.data.type;
            c.startDate = this.req.data.startdate;
            c.endDate = this.req.data.enddate;
            c.userid = this.req.data.userid;

            List<M_T_Notice> etyList = this.Select(c);

            resp.data.result = "100";
            resp.data.message = "接口操作完成";
            resp.data.noticenum = etyList.Count;            
            for (int i = 0; i < etyList.Count; i++)
            {
                Resp311_2 r311_2 = new Resp311_2();
                r311_2.date=etyList[i].CreateDate.ToString("yyyy-MM-dd");
                r311_2.detail = etyList[i].Detail;
                r311_2.enddate = Common.DateFmt(etyList[i].EndDate, "yyyy-MM-dd");
                r311_2.startdate = Common.DateFmt(etyList[i].StartDate, "yyyy-MM-dd");
                r311_2.noticeid = etyList[i].NoticeId;
                r311_2.title = etyList[i].Title;
                r311_2.type = etyList[i].Type;
                r311_2.imgurl = etyList[i].ImgUrlLarge;
                resp.data.noticeinfo.Add(r311_2);
            }
        }
        catch (Exception err)
        {
            resp.code = "200";
            resp.msg = "请求失败";
            resp.status = "false";
            resp.data.result = "200";
            resp.data.message = err.Message;
        }

        return Common.Serialize<Resp311>(resp);
    }

    private List<M_T_Notice> Select(Criteria c)
    {
        DbHelper db = new DbHelper(Common.OracleConnStrLocal, false);
        try
        {
            string sql = "select * from " + M_T_Notice.TableName+" where 1=1 ";

            if (c.startDate!="")
            {
                sql += " and startdate>=to_date('" + c.startDate + "','yyyy-mm-dd')";
            }
            if (c.endDate!="")
            {
                sql += " and enddate<=to_date('" + c.endDate + "','yyyy-mm-dd')";
            }
            if (c.userid!="")
            {
                sql += " and leaseNumber='" + c.userid + "'";
            }
            if (c.noticeType!="")
            {
                sql += " and type='" + c.noticeType + "'";
            }
            return db.ExecuteList<M_T_Notice>(sql, null);
        }
        catch (Exception err)
        {
            db.Abort();
            throw err;
        }
        finally
        {
            db.Commit();
        }
    }
}