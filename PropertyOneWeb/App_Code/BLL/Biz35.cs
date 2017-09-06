using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// 3.5	通知后台支付结果处理类
/// </summary>
/// 此接口需要定义一个账号/公司与客户编码的对应关系表，否则不能正确生成receipt和分录
public class Biz35 : BizFactory
{
    Req35 req = null;
    M_T_PAYMENT etyH = null;
    M_T_PAYMENT_INFO etyD = null;

    public Biz35()
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="postStr"></param>
    public Biz35(string postStr)
    {
        this.BIZ_NAME = "接口3.5";
        this.req = Common.Deserialize<Req35>(postStr) as Req35;
    }

    /// <summary>
    /// 发起处理请求
    /// </summary>
    /// <returns></returns>
    public override string InvokeRequest()
    {
        Resp35 resp = new Resp35(); //定义返回数据实体
        M_T_PAYMENT etyTmp = null;  //定义支付表实体.
        
        DbHelper db = new DbHelper(true);
        Receipt r = null;
        try
        {
            /// 0 在本地保存支付信息
            this.DAL_Insert_Pay();

            /// 1 创建receipt
            r = new Receipt(db);
            r.Create(req.data); //创建receipt
            db.Commit();

            etyTmp = this.DAL_Select_PayById(etyH.paymentid);
            etyTmp.status = "1"; //创建receipt完成后，状态位置1
            this.DAL_Update_Pay(etyTmp);
        }
        catch (Exception err)
        {
            db.Abort();
            resp.status = "false";
            resp.time = Common.Today;
            resp.code = "200";
            resp.msg = "接口操作失败!" + err.Message;
            return Common.Serialize<Resp35>(resp);
        }
        
        /// 2 创建distribution
        try
        {
            this.CreateDistribution(r.EtyReceipt.RECEIPT_NUMBER, r.EtyReceipt.RECEIPT_TYPE);

            etyTmp = this.DAL_Select_PayById(etyH.paymentid);
            etyTmp.status = "2";
            this.DAL_Update_Pay(etyTmp);

        }
        catch (Exception err)
        {
            resp.status = "false";
            resp.time = Common.Today;
            resp.code = "200";
            resp.msg = "Receipt创建成功！但Distribution创建失败!" + err.Message;
            return Common.Serialize<Resp35>(resp);
        }
        
        resp.status = "true";
        resp.time = Common.Today;
        resp.code = "100";
        resp.msg = "接口操作成功!";
        resp.data.result = "100";
        resp.data.message = "Receipt创建成功!";

        return Common.Serialize<Resp35>(resp);
    }

    #region 私有函数实现业务层功能
    /// <summary>
    /// 创建分录
    /// </summary>
    /// <param name="reNum"></param>
    /// <param name="reType"></param>
    private void CreateDistribution(string reNum, string reType)
    {
        DbHelper db = new DbHelper(true);
        try
        {
            COA c = new COA(db);
            ArrayList al = c.Generate_Distribution(reNum, reType);

            string strType = "R";
            if (reType == "UD")
                strType = "U";

            db.ExecuteNonQuery("delete from AR_Line_Distribution where TRANSACTION_NO='" + reNum + "' and TRANSACTION_TYPE='" + strType + "' and post_status<>'P'");
            c.Insert_Distribution(al);
            db.Commit();
        }
        catch (Exception err)
        {
            db.Abort();
            throw err;
        }
    }

    /// <summary>
    /// 创建receipt后保存支付信息
    /// </summary>
    private void DAL_Insert_Pay()
    {
        DbHelper db = new DbHelper(Common.OracleConnStrLocal, true);
        try
        {
           etyH = new M_T_PAYMENT();
           etyH.paymentid = BHelper.FetchPaymentId(db) + 1;
           etyH.amount = this.req.data.actualamount;
           etyH.leasenumber = this.req.data.leasenum;
           etyH.paydate = DateTime.Parse(this.req.data.actualpaydate);
           etyH.paytype = this.req.data.actualpaytype;
           etyH.status = "0"; // Status取值定义:0=本地保存完成;1=Receipt创建完成;2=分录创建完成。状态必须从0~2顺序转换。
           Common.Insert<M_T_PAYMENT>(etyH, db);

            Req35_2 detail = null;
            etyD = new M_T_PAYMENT_INFO();
            for (int i = 0; i < this.req.data.actualpayinfo.Count; i++)
            {
                detail = this.req.data.actualpayinfo[i];

                etyD.actualpay = detail.actualpay;
                etyD.amount = detail.amount;
                etyD.chargecode = detail.chargecode;
                etyD.invoicenumber = detail.transno;
                etyD.invoicelinenum = detail.invoicelinenum;
                etyD.paymentid = etyH.paymentid;
                Common.Insert<M_T_PAYMENT_INFO>(etyD, db);
            }
            db.Commit();
        }
        catch (Exception err)
        {
            db.Abort();
            throw err;
        }
    }

    private void DAL_Update_Pay(M_T_PAYMENT etyH)
    {
        DbHelper db = new DbHelper(Common.OracleConnStrLocal, true);
        try
        {
            List<string> keys = new List<string>();
            keys.Add("paymentid");
            Common.Update<M_T_PAYMENT>("T_Payment", etyH, db, keys);
            db.Commit();
        }
        catch(Exception err)
        {
            db.Abort();
            throw err;
        }
    }

    private M_T_PAYMENT DAL_Select_PayById(int id)
    {
        DbHelper db = new DbHelper(Common.OracleConnStrLocal, false);
        try
        {
            string sql = "select * from T_Payment where 1=1 ";
            sql += " and paymentid=" + id.ToString();

            List<M_T_PAYMENT> etyList = db.ExecuteList<M_T_PAYMENT>(sql, null);
            db.Commit();

            return etyList[0];
        }
        catch(Oracle.DataAccess.Client.OracleException err)
        {
            db.Abort();
            throw err;
        }
    }

    #endregion
}
