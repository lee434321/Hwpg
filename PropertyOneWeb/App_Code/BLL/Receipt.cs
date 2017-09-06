using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Globalization;

/// <summary>
/// Summary description for Receipt
/// </summary>
public class Receipt
{
    DbHelper _db = null;
    M_Receipt _receipt = null;

    public M_Receipt EtyReceipt
    {
        get { return _receipt; } 
    }
         
    public string ErrorMessage
    { get { return errMsg; } }
    private string errMsg = "";

	public Receipt(DbHelper db)
	{
        this._db = db;
	}

    /// <summary>
    /// 新增Receipt
    /// </summary>
    /// <param name="result">用于创建receipt的支付信息</param>
    public bool Create(Req35_1 data) 
    {
        /// 1.检查
        /// 2.新增receipt表数据
        /// 3.创建ar_application_line
        /// 4.创建分录
       
        try
        {
            #region 1.检查
            ///考虑重复创建receipt的情况
            ///
            #endregion

            #region 2.新增receipt表数据
            M_Receipt receipt = new M_Receipt();
            receipt.RECEIPT_TYPE = "NL";
            receipt.RECEIPT_DATE = "to_date('" + Common.EstrSql(Common.DateFmt(DateTime.Now, "yyyy-MM-dd")) + "','yyyy-MM-dd')"; //need
            receipt.RECEIVING_COMPANY_CODE = Common.EstrSql(data.receivecompanycode);
            receipt.PAYMENT_METHOD = Common.GetPaymentNm(data.actualpaytype);
            receipt.CURRENCY = data.currencycode;
            receipt.RATE = 1;
            receipt.AMOUNT = data.actualamount; //实际支付金额，由于不能修改,则一定等于应收金额
            receipt.BANK_ACCOUNT = data.bankaccount;
            receipt.CUSTOMER_NUMBER = data.customercode;
            receipt.SITE_NUMBER = BHelper.FetchCustSite(data.leasenum, this._db);
            receipt.VOID_DATE = "null";
            receipt.BANK_IN_DATE = "to_date('" + Common.DateFmt10(Common.EstrSql(data.actualpaydate)) + "','yyyy-MM-dd')";
            receipt.PDC_RECEIPT_DATE = "null";
            receipt.CHEQUE_NUMBER = "";
            receipt.BILLING_COMPANY_CODE = "";
            receipt.DESCRIPTION = "";
            receipt.POST_STATUS = "U";
            string maxnumber = Common.GetSingleValue("select max(RECEIPT_NUMBER) from ar_RECEIPT where RECEIPT_NUMBER like '" + data.receivecompanycode + "-" + "NL" + "%'",this._db); //receivecompanycode必须有值            
            if (maxnumber != null && maxnumber != "")
                maxnumber = maxnumber.Substring(8);
            else
                maxnumber = "0";
            int oldnumber = int.Parse(maxnumber) + 1;
            string newnum = oldnumber.ToString().PadLeft(7, '0'); //7位字符串左边补零
            receipt.RECEIPT_NUMBER = data.receivecompanycode + "-" + "NL" + "-" + newnum;
            receipt.GL_DATE = "null";
            receipt.POSTING_DATE = "null";
            receipt.APPLICATION_STATUS = "F";
            receipt.RECEIPT_STATUS = "N";
            receipt.CREATED_BY = Common.CurrentUser;
            receipt.LAST_UPDATED_BY = Common.CurrentUser;
            receipt.APPROVAL_STATUS = "U";
            receipt.ACTIVE = "A";
            receipt.IDENTIFIED_DATE = "null";
            receipt.IND_VOID_RECEIPT_ON_STATEMENT = "N";
            receipt.RECEIPT_SOURCE = "M";
            receipt.OVERPLUS_AMOUNT = 0;
            DAL_Receipt_Insert(receipt);
            this._receipt = receipt;
            #endregion

            #region 3.创建ar_application_line

            int seqNum = 1;
            Req35_2 detail =null;
            for (int i = 0; i < data.actualpayinfo.Count; i++)
            {
                detail = data.actualpayinfo[i];
                if (detail.actualpay <= 0) //如果实际支付金额<=0,则跳过。
                    continue;

                M_Application_Line Application_Line = new M_Application_Line();
                Application_Line.AMOUNT = data.actualpayinfo[i].actualpay.ToString();
                Application_Line.APPLICATION_LINE_NUMBER = seqNum.ToString(); //单据序列号
                Application_Line.CHARGE_CODE = detail.chargecode;
                Application_Line.CREATED_BY = Common.CurrentUser;
                Application_Line.FROM_RECEIPT_NUMBER = receipt.RECEIPT_NUMBER;
                Application_Line.FROM_RECEIPT_STATUS = receipt.RECEIPT_STATUS;
                Application_Line.FROM_RECEIPT_TYPE = receipt.RECEIPT_TYPE;
                Application_Line.INVOICE_LINE_NUMBER = detail.invoicelinenum.ToString();
                Application_Line.INVOICE_NUMBER = detail.transno;
                Application_Line.LAST_UPDATED_BY = Common.CurrentUser;
                Application_Line.LAST_UPDATED_DATE = "to_date('" + System.DateTime.Now.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo) + "','yyyy-MM-dd')";
                Application_Line.RECEIPT_NUMBER = receipt.RECEIPT_NUMBER;
                Application_Line.RECEIPT_STATUS = receipt.RECEIPT_STATUS;
                Application_Line.RECEIPT_TYPE = receipt.RECEIPT_TYPE;
                DAL_AppLine_Insert(Application_Line);
                
                string paystatus = "U";
                string oldoutstandingValue = Common.GetSingleValue("select outstanding_amount from ar_invoice_line where invoice_number='" + Application_Line.INVOICE_NUMBER + "'and invoice_line_number='" + Application_Line.INVOICE_LINE_NUMBER + "'", this._db);
                string outstandingValue = Convert.ToString(Convert.ToDecimal(oldoutstandingValue) - Convert.ToDecimal(Application_Line.AMOUNT));
                if (Convert.ToDecimal(outstandingValue) == 0)
                    paystatus = "F";
                else if (Convert.ToDecimal(outstandingValue) != 0 && Convert.ToDecimal(Application_Line.AMOUNT) != 0)
                    paystatus = "P";
                this.UpdateInvice(Application_Line.INVOICE_NUMBER, Application_Line.INVOICE_LINE_NUMBER, outstandingValue, paystatus);
                this._db.ExecuteNonQuery("update ar_invoice_line a set a.settle_before_due_date='Y' where a.INVOICE_NUMBER = ( select b.INVOICE_NUMBER from ar_invoice b where b.payment_due_date>=to_date('" + Common.DateFmt10(data.actualpaydate) + "','yyyy-MM-dd') and b.invoice_number='" + Application_Line.INVOICE_NUMBER + "') and a.invoice_line_number='" + Application_Line.INVOICE_LINE_NUMBER + "'");
                this._db.ExecuteNonQuery("update ar_invoice_line a set a.LAST_APPLICATION_DATE=sysdate where  a.invoice_number='" + Application_Line.INVOICE_NUMBER + "' and a.invoice_line_number='" + Application_Line.INVOICE_LINE_NUMBER + "'");
                seqNum++;
            }
            #endregion

            return true;
        }
        catch(Exception err)
        {
            throw err;
        }
    }

    /// <summary>
    /// 更新ar_invoice表
    /// </summary>
    /// <param name="InvoiceNo"></param>
    /// <param name="InvoiceLineNo"></param>
    /// <param name="outstandingValue"></param>
    /// <param name="paystatus"></param>
    private void UpdateInvice(string InvoiceNo, string InvoiceLineNo, string outstandingValue, string paystatus)
    {
        string invoiceAmount = Common.GetSingleValue("Select invoice_amount From ar_invoice_line Where invoice_number='" + InvoiceNo + "' and invoice_line_number='" + InvoiceLineNo + "'",this._db);
        if (decimal.Parse(invoiceAmount) >= 0)
        {
            if (decimal.Parse(invoiceAmount) < decimal.Parse(outstandingValue))
            {
                outstandingValue = invoiceAmount;
            }
        }
        else
        {
            if (decimal.Parse(invoiceAmount) > decimal.Parse(outstandingValue))
            {
                outstandingValue = invoiceAmount;
            }
        }
        this._db.ExecuteNonQuery("update ar_invoice_line set outstanding_amount='" + outstandingValue + "',PAY_STATUS='" + paystatus + "' where invoice_number='" + InvoiceNo + "' and invoice_line_number='" + InvoiceLineNo + "'");
    }

    /// <summary>
    /// 插入AR_Receipt表
    /// </summary>
    /// <param name="model"></param>
    public void DAL_Receipt_Insert(M_Receipt model)
    {
        StringBuilder strSql = new StringBuilder();
        strSql.Append("insert into AR_RECEIPT(");
        strSql.Append("POST_STATUS,RECEIPT_DATE,RECEIPT_NUMBER,PAYMENT_METHOD,CHEQUE_NUMBER,RECEIPT_TYPE,CUSTOMER_NUMBER,SITE_NUMBER,GL_DATE,POSTING_DATE,BANK_IN_DATE,CURRENCY,RATE,AMOUNT,DESCRIPTION,RECEIVING_COMPANY_CODE,PROPERTY,APPLICATION_STATUS,RECEIPT_STATUS,PRINT,PRINT_DATE,BANK_ACCOUNT,CREATED_BY,CREATION_DATE,LAST_UPDATED_BY,LAST_UPDATED_DATE,BILLING_COMPANY_CODE,APPROVAL_STATUS,ACTIVE,VOID_DATE,PDC_RECEIPT_DATE,IDENTIFIED_DATE,IND_VOID_RECEIPT_ON_STATEMENT,RECEIPT_SOURCE,PAYER,ORIGINAL_RECEIPT_NUMBER,OVERPLUS_AMOUNT");
        strSql.Append(")");
        strSql.Append(" values (");
        strSql.Append("'" + model.POST_STATUS + "',");
        strSql.Append("" + model.RECEIPT_DATE + ",");
        strSql.Append("'" + model.RECEIPT_NUMBER + "',");
        strSql.Append("'" + model.PAYMENT_METHOD + "',");
        strSql.Append("'" + model.CHEQUE_NUMBER + "',");
        strSql.Append("'" + model.RECEIPT_TYPE + "',");
        strSql.Append("'" + model.CUSTOMER_NUMBER.ToUpper() + "',");
        strSql.Append("'" + model.SITE_NUMBER.ToUpper() + "',");
        strSql.Append("" + model.GL_DATE + ",");
        strSql.Append("" + model.POSTING_DATE + ",");
        strSql.Append("" + model.BANK_IN_DATE + ",");
        strSql.Append("'" + model.CURRENCY + "',");
        strSql.Append("" + model.RATE + ",");
        strSql.Append("" + model.AMOUNT.ToString("###0.00") + ",");
        strSql.Append("'" + model.DESCRIPTION + "',");
        strSql.Append("'" + model.RECEIVING_COMPANY_CODE + "',");
        strSql.Append("'" + model.PROPERTY + "',");
        strSql.Append("'" + model.APPLICATION_STATUS + "',");
        strSql.Append("'" + model.RECEIPT_STATUS + "',");
        strSql.Append("" + model.PRINT + ",");
        strSql.Append("null,");
        strSql.Append("'" + model.BANK_ACCOUNT.ToUpper() + "',");
        strSql.Append("'" + model.CREATED_BY + "',");
        strSql.Append("sysdate,");
        strSql.Append("'" + model.LAST_UPDATED_BY + "',");
        strSql.Append("sysdate,");
        strSql.Append("'" + model.BILLING_COMPANY_CODE + "',");
        strSql.Append("'" + model.APPROVAL_STATUS + "',");
        strSql.Append("'" + model.ACTIVE + "',");
        strSql.Append("" + model.VOID_DATE + ",");
        strSql.Append("" + model.PDC_RECEIPT_DATE + ",");
        strSql.Append("" + model.IDENTIFIED_DATE + ",");
        strSql.Append("'" + model.IND_VOID_RECEIPT_ON_STATEMENT + "',");
        strSql.Append("'" + model.RECEIPT_SOURCE + "',");
        strSql.Append("'" + model.PAYER + "',");
        strSql.Append("'" + model.ORIGINAL_RECEIPT_NUMBER + "',");
        strSql.Append("" + model.OVERPLUS_AMOUNT + "");
        strSql.Append(")");

        _db.ExecuteNonQuery(strSql.ToString());
    }

    /// <summary>
    /// 插入AR_Application_Line表
    /// </summary>
    /// <param name="model"></param>
    private void DAL_AppLine_Insert(M_Application_Line model)
    {
        StringBuilder strSql = new StringBuilder();
        strSql.Append("insert into AR_APPLICATION_LINE(");
        strSql.Append("APPLICATION_LINE_NUMBER,RECEIPT_NUMBER,RECEIPT_TYPE,INVOICE_NUMBER,INVOICE_LINE_NUMBER,CHARGE_CODE,AMOUNT,REMARKS,FROM_RECEIPT_NUMBER,FROM_RECEIPT_TYPE,CREATED_BY,CREATION_DATE,LAST_UPDATED_BY,LAST_UPDATED_DATE,RECEIPT_STATUS,FROM_RECEIPT_STATUS");
        strSql.Append(")");
        strSql.Append(" values (");
        strSql.Append("'" + model.APPLICATION_LINE_NUMBER + "',");
        strSql.Append("'" + model.RECEIPT_NUMBER + "',");
        strSql.Append("'" + model.RECEIPT_TYPE + "',");
        strSql.Append("'" + model.INVOICE_NUMBER + "',");
        strSql.Append("'" + model.INVOICE_LINE_NUMBER + "',");
        strSql.Append("'" + model.CHARGE_CODE + "',");
        strSql.Append("'" + model.AMOUNT + "',");
        strSql.Append("'" + model.REMARKS + "',");
        strSql.Append("'" + model.FROM_RECEIPT_NUMBER + "',");
        strSql.Append("'" + model.FROM_RECEIPT_TYPE + "',");
        strSql.Append("'" + model.CREATED_BY + "',");
        strSql.Append("sysdate,");
        strSql.Append("'" + model.LAST_UPDATED_BY + "',");
        strSql.Append("sysdate,");
        strSql.Append("'" + model.RECEIPT_STATUS + "',");
        strSql.Append("'" + model.FROM_RECEIPT_STATUS + "'");
        strSql.Append(")");        
        _db.ExecuteNonQuery(strSql.ToString());
    }
}