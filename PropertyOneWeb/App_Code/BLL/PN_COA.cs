using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for PN_COA
/// </summary>
public class COA
{
    DbHelper _db = null;
	public COA(DbHelper db)
	{
        this._db = db;
	}

    public ArrayList Generate_Distribution(string reNum, string reType)
    {
        ArrayList result = new ArrayList();
        try
        {
            COARuleReceipt rule = new COARuleReceipt(this._db);
            rule.ReceiptNum = reNum;
            rule.ReceiptType = reType;
            result = rule.Generate_Receipt_Line_Distribution();
        }
        catch (Exception err)
        {
            throw err; 
        }

        return result;
    }

    public int Insert_Distribution(ArrayList arrDis)
    {
        int result = -1;
        try
        {
            if (arrDis != null)
            {
                string sqlInsert = "";
                foreach (LineDistribution tmp in arrDis)
                {
                    sqlInsert = "Insert Into AR_Line_Distribution(" +
                            " JOURNAL_LINE_NUMBER," +
                            " LINE_NO, " +
                            " TRANSACTION_TYPE, " +
                            " TRANSACTION_NO, " +
                            " AMOUNT, " +
                            " COMPANY, " +
                            " ACCOUNT, " +
                            " COST_CENTER, " +
                            " PRODUCT_CODE, " +
                            " SALES, " +
                            " COUNTRY, " +
                            " INTER_COMPANY, " +
                            " PROJECT, " +
                            " SPARE, " +
                            " DEBIT_CREDIT, " +
                            " CURRENCY, " +
                            " EXCHANGE_RATE, " +
                            " CREATED_BY, " +
                            " CREATION_DATE, " +
                            " LAST_UPDATED_BY, " +
                            " LAST_UPDATED_DATE, " +
                            " POST_STATUS, " +
                            " REMARKS ) Values(AR_Journal_Line.Nextval,'" +
                            tmp.Line_No + "', '" +
                            tmp.Transaction_Type + "', '" +
                            tmp.Transaction_No + "', '" +
                            tmp.Amount + "', '" +
                            tmp.Company + "', '" +
                            tmp.Account + "', '" +
                            tmp.Cost + "', '" +
                            tmp.Product + "', '" +
                            tmp.Sales + "', '" +
                            tmp.Country + "', '" +
                            tmp.Inter + "', '" +
                            tmp.Project + "', '" +
                            tmp.Spare + "', '" +
                            tmp.Debit_Credit + "', '" +
                            tmp.Currency + "', " +
                            tmp.Exchange_Rate + ", '" +
                            (System.Web.HttpContext.Current == null ? "Sysadmin" : Common.CurrentUser) + "', " +
                            "sysdate,'" +
                            (System.Web.HttpContext.Current == null ? "Sysadmin" : Common.CurrentUser) + "', " +
                            "sysdate," +
                            "'U','" +
                            tmp.Remark + "')";

                    this._db.ExecuteNonQuery(sqlInsert);
                }
                result = arrDis.Count;
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return result;
    }
}