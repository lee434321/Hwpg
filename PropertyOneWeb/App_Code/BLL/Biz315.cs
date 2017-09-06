using System;
using System.Collections.Generic;
using System.Web;
using System.Data;

/// <summary>
/// 接口 3.15 获取一张对账单所有信息业务处理类
/// </summary>
public class Biz315 : BizFactory
{
    Req315 req = null;
    
    public Biz315(string postStr)
    {
        this.BIZ_NAME = "接口3.15";
        this.req = Common.Deserialize<Req315>(postStr) as Req315;
    }

    public override string InvokeRequest()
    {
        Resp315 resp = new Resp315();
        try
        {
            if (this.req.data.leasenum=="" ||this.req.data.statementnum=="")
            {
                throw new Exception("租约号和账单号不能为空");
            }

            DataSet ds = this.DAL_Select_Statement();
            DataTable dtStatement = ds.Tables["Statement"];
            DataTable dtCompany = ds.Tables["Company"];
            if (dtStatement.Rows.Count > 0)
            {
                resp.data = this.Rig(ds);
            }
            else
            {
                resp.data.result = "200";
                resp.data.message = "没有找到账单数据";
            }
        
        }
        catch (Exception err)
        {
            resp.code = "200";
            resp.msg = err.Message;
            resp.status = "false";
            resp.time = Common.Today;
        }
        return Common.Serialize<Resp315>(resp);
    }

    private DataSet DAL_Select_Statement()
    {
        DataSet ds = new DataSet();

        DbHelper db = new DbHelper(Common.OracleConnStr, false);
        string sql = "";
        sql += " SELECT A.STATEMENT_NUMBER, ";
        sql += "A.STATEMENT_DATE ,  ";
        sql += "A.CUT_OFF_DATE , ";
        sql += "A.Statement_Due_Day,     ";
        sql += "A.LEASE_NUMBER , ";
        sql += "A.PREMISE  ,       ";
        sql += "A.PROPERTY_CODE,";
        sql += "A.CUSTOMER_NUMBER,";
        sql += "A.CUSTOMER_NAME ,  ";
        sql += "A.PPS_MERCHANT_CODE ,  ";
        sql += "A.ACCOUNT_NUMBER ,    ";
        sql += "A.PREVIOUS_CURRENT,   ";
        sql += "A.TRANSACTION_DATE,  ";
        sql += "nvl(A.TRANSACTION_NUMBER, ' ') TRANSACTION_NUMBER,  ";
        sql += "A.TRANSACTION_TYPE,    ";
        sql += "A.CHARGE_CODE,   ";
        sql += "A.CHARGE_GROUP,   ";
        sql += "A.CHARGE_DESCRIPTION,  ";
        sql += "A.PAYMENT_DESCRIPTION, ";
        sql += "A.PERIOD_FROM, ";
        sql += "A.PERIOD_TO, ";
        sql += "decode(A.PREVIOUS_CURRENT,'Payment/Adjustment',0-A.TRANSACTION_AMOUNT,A.TRANSACTION_AMOUNT) as TRANSACTION_AMOUNT, ";
        sql += "A.DUE_DATE , ";
        sql += "A.OVER_DUE_DAYS, ";
        sql += "A.PREMISE4 , ";
        sql += "cs.billing_address_1 contactaddress1, ";
        sql += "cs.billing_address_2 contactaddress2, ";
        sql += "cs.billing_address_3 contactaddress3, ";
        sql += "cs.billing_address_4 contactaddress4, ";
        sql += "d.description_en, ";
        sql += "d.description_ch, ";
        sql += "a.statement_seq, ";
        sql += "cs.printing_sequence,";
        sql += "a.has_sub, to_char(A.STATEMENT_DATE, 'yyyymmdd') STR_STATEMENTDATE, overdue_rate, overdue_type, overdue_prime, overdue_prime_ch, remark, statement_day ";
        sql += ",(case when length(trim(A.LEASE_NUMBER))>4 then '5097' else trim(A.LEASE_NUMBER) end) as company_code1 ";
        sql += ",vl.company_name_en as companyname ";
        sql += "FROM   (SELECT T1.STATEMENT_DATE, ";
        sql += "             T1.CUT_OFF_DATE, ";
        sql += "           T1.Statement_Due_Day, ";
        sql += "            t1.relation, ";
        sql += "            T2.* ";
        sql += "      FROM   AR_STATEMENT T1, ";
        sql += "            AR_STATEMENT_DETAIL T2 ";
        sql += "        WHERE  T1.STATEMENT_NUMBER = T2.STATEMENT_NUMBER) A ";
        sql += "inner join ar_customer_site cs on TRIM(cs.site_number) = TRIM(a.customer_number) ";
        sql += "inner join AR_CUSTOMER B on cs.CUSTOMER_NUMBER = B.CUSTOMER_NUMBER ";
        sql += "left join ad_code_description d on upper(trim(A.CHARGE_DESCRIPTION)) = upper(trim(d.code)) ";
        sql += "left join ad_charge_code acc on upper(trim(A.CHARGE_DESCRIPTION)) = upper(trim(acc.charge_code)) ";
        sql += "left join vw_ar_statement_landlord vl on a.property_code=vl.property_code ";
        sql += "WHERE ";
        sql += "1=1 ";
        sql += "and trim(lease_number) ='" + this.req.data.leasenum + "' ";
        sql += "and a.statement_seq='" + this.req.data.statementnum + "' "; //注意:这里的statementnum实际对应的是statement_seq

        DataTable dtStatement = new DataTable();
        dtStatement = db.ExecuteDT(sql); //取主表数据
        dtStatement.TableName = "Statement";
        ds.Tables.Add(dtStatement);

        if (dtStatement.Rows.Count>0)
        {
            sql = "select * from vw_ar_statement_landlord where property_code='" + dtStatement.Rows[0]["PROPERTY_CODE"] + "'";
            DataTable dtCompany = new DataTable();
            dtCompany = db.ExecuteDT(sql);   //取公司信息
            dtCompany.TableName = "Company";
            ds.Tables.Add(dtCompany);
        }
        db.Commit();
        return ds;
    }

    private DataTable DAL_Select_Payment(string statementNum,string leaseNum,string receiptNum)
    {
        DbHelper db = new DbHelper(Common.OracleConnStr, false);
        string sql = "";
        sql = "select * ";
        sql += "from ar_statement_detail_items di ";
        sql += "left join ad_charge_code acc ";
        sql += "on di.charge_code = acc.charge_code ";
        sql += "where statement_number = '" + statementNum + "' ";
        sql += "and receipt_number = '" + receiptNum + "' ";
        sql += "and lease_no = '" + leaseNum + "' ";
        sql += "order by invoice_date,";
        sql += "invoice_number,";
        sql += "acc.display_sequence,";
        sql += "description_en ";

        DataTable dtPayment = db.ExecuteDT(sql);
        db.Commit();

        return dtPayment;
    }

    private Resp3151 Rig(DataSet ds)
    {
        Resp3151 r3151 = new Resp3151();
        DataTable dtStatement = ds.Tables["Statement"];
        DataTable dtCompany = ds.Tables["Company"];
        DataTable dtStatementDetail = ds.Tables["StatementDetail"];

        r3151.ppsmerchantcode = Common.DbNull2Str(dtStatement.Rows[0]["PPS_MERCHANT_CODE"]);
        r3151.accountnumber = Common.DbNull2Str(dtStatement.Rows[0]["ACCOUNT_NUMBER"]);
        r3151.companyname = Common.DbNull2Str(dtCompany.Rows[0]["COMPANY_NAME_EN"]);
        r3151.contactaddress1 = Common.DbNull2Str(dtStatement.Rows[0]["contactaddress1"]);
        r3151.contactaddress2 = Common.DbNull2Str(dtStatement.Rows[0]["contactaddress2"]);
        r3151.contactaddress3 = Common.DbNull2Str(dtStatement.Rows[0]["contactaddress3"]);
        r3151.contactaddress4 = Common.DbNull2Str(dtStatement.Rows[0]["contactaddress4"]);
        r3151.customername = Common.DbNull2Str(dtStatement.Rows[0]["CUSTOMER_NAME"]);
        r3151.cutoffdate = Common.DbNull2Str(dtStatement.Rows[0]["CUT_OFF_DATE"]);
        r3151.duedate = Common.DbNull2Str(dtStatement.Rows[0]["DUE_DATE"]);
        r3151.leasenumber = Common.DbNull2Str(dtStatement.Rows[0]["LEASE_NUMBER"]);
        r3151.overdueprime = Common.DbNull2Str(dtStatement.Rows[0]["overdue_prime"]);     //逾期付款方式
        r3151.overdueprimech = Common.DbNull2Str(dtStatement.Rows[0]["overdue_prime_ch"]);  //逾期付款方式中文
        r3151.premise = Common.DbNull2Str(dtStatement.Rows[0]["PREMISE"]);
        r3151.premise4 = Common.DbNull2Str(dtStatement.Rows[0]["PREMISE4"]);
        r3151.statementdate = Common.DbNull2Str(dtStatement.Rows[0]["STATEMENT_DATE"]);
        r3151.statementnumber = Common.DbNull2Str(dtStatement.Rows[0]["statement_seq"]);

        //取Previous balance
        r3151.previousbalance = Common.DbNull2Dec(dtStatement.Compute("sum(TRANSACTION_AMOUNT)", "TRANSACTION_TYPE='1'"));

        //取Payment/Adjustment金额
        DataRow[] drPay = dtStatement.Select("TRANSACTION_TYPE='2' and CHARGE_CODE is null"); //取支付主信息
        if (drPay.Length > 0)
        {
            for (int i = 0; i < drPay.Length; i++)
            {
                Resp31522 r31522 = new Resp31522();
                r31522.paymentamount = Common.DbNull2Dec(drPay[i]["TRANSACTION_AMOUNT"]);
                r31522.paymentchargedesc = Common.DbNull2Str(drPay[i]["description_en"]) + " " + Common.DbNull2Str(drPay[i]["description_ch"]);
                r31522.paymentdate = Common.DbNull2Str(drPay[i]["TRANSACTION_DATE"]);   
                r31522.paymentdesc = Common.DbNull2Str(drPay[i]["PAYMENT_DESCRIPTION"]);
                r31522.paymenttrans = Common.DbNull2Str(drPay[i]["TRANSACTION_NUMBER"]);

                //取历史已支付信息
                DataTable dtPayHis = this.DAL_Select_Payment(dtStatement.Rows[0]["STATEMENT_NUMBER"].ToString(), r3151.leasenumber, r31522.paymenttrans);
                if (dtPayHis.Rows.Count>0)
                {
                    for (int j = 0; j < dtPayHis.Rows.Count; j++)
                    {
                        Resp31531 r31531 = new Resp31531();
                        r31531.historyamount = Common.DbNull2Dec(dtPayHis.Rows[j]["AMOUNT"]);
                        r31531.historychargedesc = "-" + Common.DbNull2Str(dtPayHis.Rows[j]["DESCRIPTION_EN"]) + " " + Common.DbNull2Str(dtPayHis.Rows[j]["DESCRIPTION_CN"]);
                        r31531.historypaymentdesc = Common.DbNull2Str(dtPayHis.Rows[j]["PERIOD"]);
                        r31522.paymenthistory.Add(r31531);
                    }
                }
                r3151.paymentinfo.Add(r31522);
            }
        }
        //取已经支付的总金额
        r3151.payment = Common.DbNull2Dec(dtStatement.Compute("sum(TRANSACTION_AMOUNT)", "TRANSACTION_TYPE='2' and CHARGE_DESCRIPTION='Payment'"));
        //取已经调整的总金额
        r3151.adjustment = Common.DbNull2Dec(dtStatement.Compute("sum(TRANSACTION_AMOUNT)", "TRANSACTION_TYPE='2' and CHARGE_DESCRIPTION='Adjustment'"));
        //取current due本期应付金额
        r3151.currentdue = Common.DbNull2Dec(dtStatement.Compute("sum(TRANSACTION_AMOUNT)", "TRANSACTION_TYPE='2' and CHARGE_CODE<>'' and CHARGE_CODE<>'INTEREST' "));
        //取overdue interest金额
        r3151.overdueinterest = Common.DbNull2Dec(dtStatement.Compute("sum(TRANSACTION_AMOUNT)", "TRANSACTION_TYPE='2' and CHARGE_CODE='INTEREST' "));
        //取以上各项合计
        r3151.total = r3151.previousbalance + r3151.payment + r3151.adjustment + r3151.currentdue + r3151.overdueinterest;
        
        r3151.statementbalance = Common.DbNull2Dec(dtStatement.Compute("sum(TRANSACTION_AMOUNT)", "TRANSACTION_TYPE='3'"));
        
        //-- start 处理当前明细 --
        List<Resp31521> cur = new List<Resp31521>();
        DataRow[] drCur = dtStatement.Select("TRANSACTION_TYPE='2' and CHARGE_CODE<>''"); //过滤出当前明细
        for (int i = 0; i < drCur.Length; i++)
        {
            Resp31521 r31521 = new Resp31521();
            r31521.chargedesc = Common.DbNull2Str(drCur[i]["description_en"]) + " " + Common.DbNull2Str(drCur[i]["description_ch"]) + "\r\n" + Common.DbNull2Str(drCur[i]["remark"]);
            r31521.paymentdesc = Common.DbNull2Str(drCur[i]["PAYMENT_DESCRIPTION"]);
            r31521.transactionamount = Common.DbNull2Dec(drCur[i]["TRANSACTION_AMOUNT"]);
            r31521.transactiondate = Common.DbNull2Str(drCur[i]["TRANSACTION_DATE"]);
            r31521.transactionno = Common.DbNull2Str(drCur[i]["TRANSACTION_NUMBER"]);
            cur.Add(r31521);
        }
        r3151.transactioninfo = cur;   //本期付款项目
        //-- end --

        r3151.result = "100";
        r3151.message = "接口操作完成";
        return r3151;
    }
}
