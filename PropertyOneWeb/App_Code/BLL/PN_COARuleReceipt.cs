using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web;
using Oracle.DataAccess.Client;

/// <summary>
/// Summary description for COARuleReceipt
/// </summary>
public class COARuleReceipt
{
    public const string RECEIPT_TYPE_NL = "NL";
    public const string RECEIPT_TYPE_AJ = "AJ";
    public const string RECEIPT_TYPE_UD = "UD";
    public const string RECEIPT_TYPE_ID = "ID";
    public const string RECEIPT_TYPE_MS = "MS";

    private ArrayList LineDis;
    private DbHelper _db=null;

    public string ReceiptNum;
    public string ReceiptType;
    
	public COARuleReceipt(DbHelper db)
	{
        this._db=db;
        this.LineDis = new ArrayList();
	}

    /// <summary>
    /// 生成Receipt分录
    /// </summary>
    /// <returns></returns>
    public ArrayList Generate_Receipt_Line_Distribution()
    {
        ArrayList result = new ArrayList();
        /// 1.获取receipt基本信息
        M_Receipt etyReceipt = Common.GetReceiptByNum(this.ReceiptNum, this._db);
        /// 2.根据receipt类型生成distribution数据

        switch (this.ReceiptType)
        {
            case RECEIPT_TYPE_NL:
                result = this.Generate_NL_Receipt_Distribution(etyReceipt);
                break;

            default:
                result = new ArrayList();
                break;
        }
        return result;
    }

    private ArrayList Generate_NL_Receipt_Distribution(M_Receipt clsReceipt)
    {
        ArrayList result = new ArrayList();
        bool A1, A2, A3, A4, A5, A6, A7;

        try
        {
            LineDistribution tmpDis = new LineDistribution();
            Segment tmpSeg = new Segment(this._db);
            string Account_Id = "";
            //此Receipt是否进行了冲消
            bool hasApplied = this.hasApplied(clsReceipt.RECEIPT_NUMBER);
            //此Receipt冲消时是否使用了Prepay
            bool hasAppliedPrepay = this.hasAppliedPrepay(clsReceipt.RECEIPT_NUMBER);
            if (hasApplied && !hasAppliedPrepay)
            {
                // 如果Receipt有冲消,并且没有使用Prepay
                #region invoice offset coa mapping (not included applied receipt case)
                #region get all the application line with this receipt about invoice offset
                string sqlstr = "Select DISTINCT Trim(A.Receipt_Number) as Receipt_Number," +
                                            " Trim(A.RECEIVING_COMPANY_CODE) as RECEIVING_COMPANY_CODE," +
                                            " Trim(B.APPLICATION_LINE_NUMBER) as APPLICATION_LINE_NUMBER," +
                    //03/07/2015 10:46 RE: PropertyOne - major enhancement邮件要求
                                            " Trim(A.Customer_Number) as Customer_Number, " +
                                            " Trim(A.Currency) as Currency, " +
                                            " A.Amount as Amount," +
                                            " A.Rate as Rate," +
                                            " Trim(A.BANK_ACCOUNT) as BANK_ACCOUNT, " +
                                            " Trim(B.Invoice_Number) as Invoice_Number," +
                                            " Trim(B.Invoice_Line_Number) as Invoice_Line_Number," +
                                            " Trim(B.Charge_Code) as Charge_Code," +
                                            " B.Amount as App_Amount," +
                                            " Trim(C.Lease_Number) as Lease_Number," +
                                            " Trim(C.BILLING_COMPANY) as Billing_Company " +
                                            "  From AR_Receipt A, AR_Application_Line B, AR_Invoice C, AR_Invoice_Line D " +
                                            " Where Upper(TRIM(A.Receipt_Number)) = Upper(TRIM(B.Receipt_Number)) AND " +
                                            " Upper(TRIM(B.Invoice_Number)) = Upper(TRIM(C.Invoice_Number)) AND " +
                                            " Upper(TRIM(B.Invoice_Line_Number)) = Upper(TRIM(D.Invoice_Line_Number)) And" +
                                            " Upper(TRIM(C.Invoice_Number)) = Upper(TRIM(D.Invoice_Number)) AND " +
                                            " Upper(TRIM(A.Receipt_Type)) = 'NL' AND " +
                                            " Upper(TRIM(B.Receipt_Number)) = Upper('" + clsReceipt.RECEIPT_NUMBER.Trim() + "')";
                DataTable dt = this._db.ExecuteDT(sqlstr);
                #endregion

                A1 = A2 = A3 = A4 = A5 = A6 = A7 = false;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    // check whether having lease
                    //03/07/2015 10:46 RE: PropertyOne - major enhancement邮件要求
                    string strCustomer = dt.Rows[i]["Customer_Number"].ToString().Trim();
                    string strInternalGroup = Check_Group(strCustomer) ? "Y" : "N"; //pong COA
                    string leaseNumber = dt.Rows[i]["Lease_Number"].ToString().Trim();
                    string receiptCompany = dt.Rows[i]["RECEIVING_COMPANY_CODE"].ToString().Trim();
                    string billingCompany = dt.Rows[i]["Billing_Company"].ToString().Trim();
                    string bankAccount = dt.Rows[i]["BANK_ACCOUNT"].ToString().Trim();
                    string strChargeCode = dt.Rows[i]["Charge_Code"].ToString().Trim();
                    double dblAppAmount = System.Convert.ToDouble(dt.Rows[i]["App_Amount"].ToString());
                    string strLineNo = dt.Rows[i]["APPLICATION_LINE_NUMBER"].ToString().Trim();

                    if (leaseNumber.Length > 0)
                    {
                        #region leased invoice offset coa mapping
                        // having lease;
                        string sqlLandlord = "Select A.LEASE_VERSION,Trim(A.Lease_Number), trim(B.Landlord_Code) as Landlord_Code,trim(B.Property_Code) as Property_Code,B.Ratio as Ratio,B.Active as Active  " +
                            //                    " From LM_Lease A,PD_Property_Landlord B," +
                            //                    " (Select Max(A.LEASE_VERSION) as LEASE_VERSION, Trim(A.Lease_Number) as Lease_Number From LM_Lease A group by A.Lease_Number) C " +
                            //                    " Where Upper(TRIM(A.Property_Code)) = Upper(Trim(B.Property_Code)) AND " +
                            //                            " Upper(Trim(A.Lease_Number)) = Upper(Trim(C.Lease_Number)) and " +
                            //                            " A.Lease_Version = C.Lease_Version And " +
                            //                            " Upper(Trim(B.Active)) = 'A' And " +
                            //                            " Upper(TRIM(A.Lease_Number)) = Upper('" + leaseNumber.Trim() + "')";
                            // Enhance the performance, Modified by Derek on Nov 20th, 2014
                                            " From LM_Lease A,PD_Property_Landlord B " +
                                            " Where Upper(TRIM(A.Lease_Number)) = Upper('" + leaseNumber.Trim() + "') AND " +
                                            " A.Status = 'A' And " +
                                            " A.Active = 'A' And " +
                                            " Upper(TRIM(A.Property_Code)) = Upper(Trim(B.Property_Code)) AND " +
                                            " Upper(Trim(B.Active)) = 'A' ";
                        sqlLandlord += " Order By Ratio";
                        DataTable dtLandlord = this._db.ExecuteDT(sqlLandlord);

                        double totalAmount = 0;
                        for (int j = 0; j < dtLandlord.Rows.Count; j++)
                        {
                            string strLandlord = dtLandlord.Rows[j]["Landlord_Code"].ToString().Trim();
                            string strProperty = dtLandlord.Rows[j]["Property_Code"].ToString().Trim();
                            //补四舍五入差额到最后一条
                            double theAmount = Common.MyRound(dblAppAmount * Double.Parse((dtLandlord.Rows[j]["Ratio"].ToString())) / 100.00, 2);
                            totalAmount += theAmount;
                            if (j == dtLandlord.Rows.Count - 1)
                            {
                                theAmount += dblAppAmount - totalAmount;
                            }

                            if (receiptCompany == strLandlord)
                            {
                                #region Receipt 2.1.1	Billing Company = Landlord Company = Receipt Company (Or Billing Com = EMPTY; Landlord Com=Receipt Com)
                                // Dr, Landlord Company, Bank(With Lease), Account Id=103
                                Account_Id = "103";
                                tmpSeg.Seg_Company = receiptCompany;
                                A1 = tmpSeg.GetAccount(Account_Id, "", "", "", "", "", receiptCompany, bankAccount, "");
                                A2 = tmpSeg.GetCostCenter(Account_Id, "", strProperty, "", "", "", receiptCompany, bankAccount);
                                A3 = tmpSeg.GetProduct(Account_Id, "", "", "", "", "", receiptCompany, bankAccount, "");
                                A4 = tmpSeg.GetCountry(Account_Id, "", "", "", receiptCompany, bankAccount, "");
                                A5 = tmpSeg.GetInter();
                                A7 = tmpSeg.GetSales(Account_Id, "", "", "", "", "", receiptCompany, bankAccount, "");
                                if (A1 && A2 && A3 && A4 && A5 && A7)
                                {
                                    SetLineDis(clsReceipt, tmpSeg, "R", "DR", theAmount, strLineNo);
                                }
                                // Cr, Landlord Company, Debtor, Account Id = 101
                                Account_Id = "101";
                                tmpSeg.Seg_Company = strLandlord;
                                A1 = tmpSeg.GetAccount(Account_Id, strLandlord, strProperty, strChargeCode, "", "", "", "", "", strInternalGroup);  //Pong COA
                                A2 = tmpSeg.GetCostCenter(Account_Id, strLandlord, strProperty, strChargeCode, "", "", "", "");
                                A3 = tmpSeg.GetProduct(Account_Id, strLandlord, strProperty, strChargeCode, "", "", "", "", "");
                                A4 = tmpSeg.GetCountry();
                                //03/07/2015 10:46 RE: PropertyOne - major enhancement邮件要求
                                //A5 = tmpSeg.GetInter();
                                if (Check_Internal(strCustomer))
                                {
                                    tmpSeg.Seg_Inter = strCustomer.Substring(1, 4);
                                    A5 = true;
                                }
                                else
                                {
                                    A5 = tmpSeg.GetInter();
                                }
                                A7 = tmpSeg.GetSales();
                                if (A1 && A2 && A3 && A4 && A5 && A7)
                                {
                                    SetLineDis(clsReceipt, tmpSeg, "R", "CR", theAmount, strLineNo);
                                }
                                #endregion
                            }
                            else if (billingCompany == strLandlord && receiptCompany != strLandlord)
                            {
                                #region Receipt 2.1.2	Billing Company = Landlord Company ≠ Receipt Company
                                // Receipt Company, (DR)Bank, Account id=302
                                Account_Id = "302";
                                tmpSeg.Seg_Company = receiptCompany;
                                A1 = tmpSeg.GetAccount(Account_Id, "", "", "", "", "", receiptCompany, bankAccount, "");
                                A2 = tmpSeg.GetCostCenter(Account_Id, "", strProperty, "", "", "", receiptCompany, bankAccount);
                                A3 = tmpSeg.GetProduct(Account_Id, "", "", "", "", "", receiptCompany, bankAccount, "");
                                A4 = tmpSeg.GetCountry(Account_Id, "", "", "", receiptCompany, bankAccount, "");
                                // A5 = tmpSeg.GetInter(Account_Id, strLandlord, "", "", "");
                                A5 = true;
                                //tmpSeg.Seg_Inter = strLandlord.Trim();
                                tmpSeg.Seg_Inter = GetInterCodeForLandlord(strLandlord.Trim());
                                A7 = tmpSeg.GetSales(Account_Id, "", "", "", "", "", receiptCompany, bankAccount, "");
                                if (A1 && A2 && A3 && A4 && A5 && A7)
                                {
                                    SetLineDis(clsReceipt, tmpSeg, "R", "DR", theAmount, strLineNo);
                                }
                                // // Receipt Company, (CR)C/A with landlord, Account id=309
                                Account_Id = "309";
                                tmpSeg.Seg_Company = receiptCompany;
                                A1 = tmpSeg.GetAccount(Account_Id, strLandlord, "", "", "", "", receiptCompany, "", "");
                                A2 = tmpSeg.GetCostCenter(Account_Id, strLandlord, "", "", "", "", receiptCompany, "");
                                A3 = tmpSeg.GetProduct(Account_Id, strLandlord, "", "", "", "", receiptCompany, "", "");
                                A4 = tmpSeg.GetCountry();
                                // A5 = tmpSeg.GetInter(Account_Id, strLandlord, "", "", "");
                                A5 = true;
                                //tmpSeg.Seg_Inter = strLandlord.Trim();
                                tmpSeg.Seg_Inter = GetInterCodeForLandlord(strLandlord.Trim());
                                A7 = tmpSeg.GetSales();
                                if (A1 && A2 && A3 && A4 && A5 && A7)
                                {
                                    SetLineDis(clsReceipt, tmpSeg, "R", "CR", theAmount, strLineNo);
                                }
                                // Landlord Company
                                // (DR)C/A with Receipt Com, Account id=104
                                Account_Id = "104";
                                tmpSeg.Seg_Company = strLandlord;
                                A1 = tmpSeg.GetAccount(Account_Id, strLandlord, "", "", "", "", receiptCompany, "", "");
                                A2 = tmpSeg.GetCostCenter(Account_Id, strLandlord, "", "", "", "", receiptCompany, "");
                                A3 = tmpSeg.GetProduct(Account_Id, strLandlord, "", "", "", "", receiptCompany, "", "");
                                A4 = tmpSeg.GetCountry();
                                // A5 = tmpSeg.GetInter(Account_Id, "", "", receiptCompany, "");
                                A5 = true;
                                tmpSeg.Seg_Inter = receiptCompany.Trim();
                                A7 = tmpSeg.GetSales();
                                if (A1 && A2 && A3 && A4 && A5 && A7)
                                {
                                    SetLineDis(clsReceipt, tmpSeg, "R", "DR", theAmount, strLineNo);
                                }
                                // (CR)Debtor, Account id=101
                                Account_Id = "101";
                                tmpSeg.Seg_Company = strLandlord;
                                A1 = tmpSeg.GetAccount(Account_Id, strLandlord, strProperty, strChargeCode, "", "", "", "", "", strInternalGroup);  //Pong COA
                                A2 = tmpSeg.GetCostCenter(Account_Id, strLandlord, strProperty, strChargeCode, "", "", "", "");
                                A3 = tmpSeg.GetProduct(Account_Id, strLandlord, strProperty, strChargeCode, "", "", "", "", "");
                                A4 = tmpSeg.GetCountry();
                                //03/07/2015 10:46 RE: PropertyOne - major enhancement邮件要求
                                //A5 = tmpSeg.GetInter();
                                if (Check_Internal(strCustomer))
                                {
                                    tmpSeg.Seg_Inter = strCustomer.Substring(1, 4);
                                    A5 = true;
                                }
                                else
                                {
                                    A5 = tmpSeg.GetInter();
                                }
                                A7 = tmpSeg.GetSales();
                                if (A1 && A2 && A3 && A4 && A5 && A7)
                                {
                                    SetLineDis(clsReceipt, tmpSeg, "R", "CR", theAmount, strLineNo);
                                }
                                #endregion
                            }
                            else if (billingCompany == receiptCompany && receiptCompany != strLandlord && !Check_Wholly_Owned_Property(strProperty))
                            {
                                #region Receipt 2.1.3	Billing Company = Receipt Company ≠ Landlord Company and Landlord is not wholly-owned subsidiary
                                // Receipt Company, (DR)Bank, Account id=302
                                Account_Id = "302";
                                tmpSeg.Seg_Company = receiptCompany;
                                A1 = tmpSeg.GetAccount(Account_Id, "", "", "", "", "", receiptCompany, bankAccount, "");
                                A2 = tmpSeg.GetCostCenter(Account_Id, "", strProperty, "", "", "", receiptCompany, bankAccount);
                                A3 = tmpSeg.GetProduct(Account_Id, "", "", "", "", "", receiptCompany, bankAccount, "");
                                A4 = tmpSeg.GetCountry(Account_Id, "", "", "", receiptCompany, bankAccount, "");
                                // A5 = tmpSeg.GetInter(Account_Id, strLandlord, "", "", "");
                                A5 = true;
                                //tmpSeg.Seg_Inter = strLandlord.Trim();
                                tmpSeg.Seg_Inter = GetInterCodeForLandlord(strLandlord.Trim());
                                A7 = tmpSeg.GetSales(Account_Id, "", "", "", "", "", receiptCompany, bankAccount, "");
                                if (A1 && A2 && A3 && A4 && A5 && A7)
                                {
                                    SetLineDis(clsReceipt, tmpSeg, "R", "DR", theAmount, strLineNo);
                                }
                                // // Receipt Company, (CR)C/A with landlord, Account id=309
                                Account_Id = "309";
                                tmpSeg.Seg_Company = receiptCompany;
                                A1 = tmpSeg.GetAccount(Account_Id, strLandlord, "", "", "", "", receiptCompany, "", "");
                                A2 = tmpSeg.GetCostCenter(Account_Id, strLandlord, "", "", "", "", receiptCompany, "");
                                A3 = tmpSeg.GetProduct(Account_Id, strLandlord, "", "", "", "", receiptCompany, "", "");
                                A4 = tmpSeg.GetCountry();
                                // A5 = tmpSeg.GetInter(Account_Id, strLandlord, "", "", "");
                                A5 = true;
                                //tmpSeg.Seg_Inter = strLandlord.Trim();
                                tmpSeg.Seg_Inter = GetInterCodeForLandlord(strLandlord.Trim());
                                A7 = tmpSeg.GetSales();
                                if (A1 && A2 && A3 && A4 && A5 && A7)
                                {
                                    SetLineDis(clsReceipt, tmpSeg, "R", "CR", theAmount, strLineNo);
                                }
                                // Landlord Company
                                // (DR)C/A with Receipt Com, Account id=104
                                Account_Id = "104";
                                tmpSeg.Seg_Company = strLandlord;
                                A1 = tmpSeg.GetAccount(Account_Id, strLandlord, "", "", "", "", receiptCompany, "", "");
                                A2 = tmpSeg.GetCostCenter(Account_Id, strLandlord, "", "", "", "", receiptCompany, "");
                                A3 = tmpSeg.GetProduct(Account_Id, strLandlord, "", "", "", "", receiptCompany, "", "");
                                A4 = tmpSeg.GetCountry();
                                // A5 = tmpSeg.GetInter(Account_Id, "", "", receiptCompany, "");
                                A5 = true;
                                tmpSeg.Seg_Inter = receiptCompany.Trim();
                                A7 = tmpSeg.GetSales();
                                if (A1 && A2 && A3 && A4 && A5 && A7)
                                {
                                    SetLineDis(clsReceipt, tmpSeg, "R", "DR", theAmount, strLineNo);
                                }
                                // (CR)Debtor, Account id=101
                                Account_Id = "101";
                                tmpSeg.Seg_Company = strLandlord;
                                A1 = tmpSeg.GetAccount(Account_Id, strLandlord, strProperty, strChargeCode, "", "", "", "", "", strInternalGroup);  //Pong COA
                                A2 = tmpSeg.GetCostCenter(Account_Id, strLandlord, strProperty, strChargeCode, "", "", "", "");
                                A3 = tmpSeg.GetProduct(Account_Id, strLandlord, strProperty, strChargeCode, "", "", "", "", "");
                                A4 = tmpSeg.GetCountry();
                                //03/07/2015 10:46 RE: PropertyOne - major enhancement邮件要求
                                //A5 = tmpSeg.GetInter();
                                if (Check_Internal(strCustomer))
                                {
                                    tmpSeg.Seg_Inter = strCustomer.Substring(1, 4);
                                    A5 = true;
                                }
                                else
                                {
                                    A5 = tmpSeg.GetInter();
                                }
                                A7 = tmpSeg.GetSales();
                                if (A1 && A2 && A3 && A4 && A5 && A7)
                                {
                                    SetLineDis(clsReceipt, tmpSeg, "R", "CR", theAmount, strLineNo);
                                }
                                #endregion
                            }
                            else if (billingCompany == receiptCompany && receiptCompany != strLandlord && Check_Wholly_Owned_Property(strProperty))
                            {
                                #region Receipt 2.1.3	Billing Company = Receipt Company ≠ Landlord Company and Landlord is wholly-owned subsidiary
                                // Receipt Company, (DR)Bank, Account id=302
                                Account_Id = "302";
                                tmpSeg.Seg_Company = receiptCompany;
                                A1 = tmpSeg.GetAccount(Account_Id, "", "", "", "", "", receiptCompany, bankAccount, "");
                                A2 = tmpSeg.GetCostCenter();
                                A3 = tmpSeg.GetProduct(Account_Id, "", "", "", "", "", receiptCompany, bankAccount, "");
                                A4 = tmpSeg.GetCountry(Account_Id, "", "", "", receiptCompany, bankAccount, "");
                                A5 = tmpSeg.GetInter();
                                A7 = tmpSeg.GetSales(Account_Id, "", "", "", "", "", receiptCompany, bankAccount, "");
                                if (A1 && A2 && A3 && A4 && A5 && A7)
                                {
                                    SetLineDis(clsReceipt, tmpSeg, "R", "DR", theAmount, strLineNo);
                                }
                                // Receipt Company, (CR)C/A with landlord, Account id=309
                                Account_Id = "309";
                                tmpSeg.Seg_Company = receiptCompany;
                                A1 = tmpSeg.GetAccount(Account_Id, strLandlord, "", "", "", "", receiptCompany, "", "");
                                A2 = tmpSeg.GetCostCenter(Account_Id, strLandlord, "", "", "", "", receiptCompany, "");
                                A3 = tmpSeg.GetProduct(Account_Id, strLandlord, "", "", "", "", receiptCompany, "", "");
                                A4 = tmpSeg.GetCountry();
                                A5 = true;
                                tmpSeg.Seg_Inter = GetInterCodeForLandlord(strLandlord.Trim());
                                A7 = tmpSeg.GetSales();
                                if (A1 && A2 && A3 && A4 && A5 && A7)
                                {
                                    SetLineDis(clsReceipt, tmpSeg, "R", "CR", theAmount, strLineNo);
                                }
                                // Receipt Company, (DR)C/A with Landlord Company, Account id=309
                                Account_Id = "309";
                                tmpSeg.Seg_Company = receiptCompany;
                                A1 = tmpSeg.GetAccount(Account_Id, strLandlord, "", "", "", "", receiptCompany, "", "");
                                A2 = tmpSeg.GetCostCenter(Account_Id, strLandlord, "", "", "", "", receiptCompany, "");
                                A3 = tmpSeg.GetProduct(Account_Id, strLandlord, "", "", "", "", receiptCompany, "", "");
                                A4 = tmpSeg.GetCountry();
                                A5 = true;
                                tmpSeg.Seg_Inter = GetInterCodeForLandlord(strLandlord.Trim());
                                A7 = tmpSeg.GetSales();
                                if (A1 && A2 && A3 && A4 && A5 && A7)
                                {
                                    SetLineDis(clsReceipt, tmpSeg, "R", "DR", theAmount, strLineNo);
                                }
                                // Receipt Company, (CR)C/A with Parent Company, Account id=305
                                Account_Id = "305";
                                tmpSeg.Seg_Company = receiptCompany;
                                A1 = tmpSeg.GetAccount(Account_Id, strLandlord, "", "", "", "", receiptCompany, "", "");
                                A2 = tmpSeg.GetCostCenter(Account_Id, strLandlord, "", "", "", "", receiptCompany, "");
                                A3 = tmpSeg.GetProduct(Account_Id, strLandlord, "", "", "", "", receiptCompany, "", "");
                                A4 = tmpSeg.GetCountry();
                                //A5 = tmpSeg.GetInter(Account_Id, strLandlord, "", receiptCompany, "");
                                A5 = true;
                                tmpSeg.Seg_Inter = GetParentCom(receiptCompany);
                                A7 = tmpSeg.GetSales();
                                if (A1 && A2 && A3 && A4 && A5 && A7)
                                {
                                    SetLineDis(clsReceipt, tmpSeg, "R", "CR", theAmount, strLineNo);
                                }
                                // Landlord Company
                                // (DR)C/A with Receipt Com, Account id=104
                                Account_Id = "104";
                                tmpSeg.Seg_Company = strLandlord;
                                A1 = tmpSeg.GetAccount(Account_Id, strLandlord, "", "", "", "", receiptCompany, "", "");
                                A2 = tmpSeg.GetCostCenter(Account_Id, strLandlord, "", "", "", "", receiptCompany, "");
                                A3 = tmpSeg.GetProduct(Account_Id, strLandlord, "", "", "", "", receiptCompany, "", "");
                                A4 = tmpSeg.GetCountry();
                                A5 = true;
                                tmpSeg.Seg_Inter = receiptCompany.Trim();
                                A7 = tmpSeg.GetSales();
                                if (A1 && A2 && A3 && A4 && A5 && A7)
                                {
                                    SetLineDis(clsReceipt, tmpSeg, "R", "DR", theAmount, strLineNo);
                                }
                                // (CR)Debtor, Account id=101
                                Account_Id = "101";
                                tmpSeg.Seg_Company = strLandlord;
                                A1 = tmpSeg.GetAccount(Account_Id, strLandlord, strProperty, strChargeCode, "", "", "", "", "", strInternalGroup);  //Pong COA
                                A2 = tmpSeg.GetCostCenter(Account_Id, strLandlord, strProperty, strChargeCode, "", "", "", "");
                                A3 = tmpSeg.GetProduct(Account_Id, strLandlord, strProperty, strChargeCode, "", "", "", "", "");
                                A4 = tmpSeg.GetCountry();
                                //03/07/2015 10:46 RE: PropertyOne - major enhancement邮件要求
                                //A5 = tmpSeg.GetInter();
                                if (Check_Internal(strCustomer))
                                {
                                    tmpSeg.Seg_Inter = strCustomer.Substring(1, 4);
                                    A5 = true;
                                }
                                else
                                {
                                    A5 = tmpSeg.GetInter();
                                }
                                A7 = tmpSeg.GetSales();
                                if (A1 && A2 && A3 && A4 && A5 && A7)
                                {
                                    SetLineDis(clsReceipt, tmpSeg, "R", "CR", theAmount, strLineNo);
                                }

                                // (DR)Landlord Company, C/A with Parent Company, Account id=105
                                Account_Id = "105";
                                tmpSeg.Seg_Company = strLandlord;
                                A1 = tmpSeg.GetAccount(Account_Id, strLandlord, "", "", "", "", receiptCompany, "", "");
                                A2 = tmpSeg.GetCostCenter(Account_Id, strLandlord, "", "", "", "", receiptCompany, "");
                                A3 = tmpSeg.GetProduct(Account_Id, strLandlord, "", "", "", "", receiptCompany, "", "");
                                A4 = tmpSeg.GetCountry();
                                //A5 = tmpSeg.GetInter(Account_Id, strLandlord, "", receiptCompany, "");
                                A5 = true;
                                tmpSeg.Seg_Inter = GetParentCom(strLandlord);
                                A7 = tmpSeg.GetSales();
                                if (A1 && A2 && A3 && A4 && A5 && A7)
                                {
                                    SetLineDis(clsReceipt, tmpSeg, "R", "DR", theAmount, strLineNo);
                                }
                                // (DR)Landlord Company C/A with Receipt Com, Account id=104
                                Account_Id = "104";
                                tmpSeg.Seg_Company = strLandlord;
                                A1 = tmpSeg.GetAccount(Account_Id, strLandlord, "", "", "", "", receiptCompany, "", "");
                                A2 = tmpSeg.GetCostCenter(Account_Id, strLandlord, "", "", "", "", receiptCompany, "");
                                A3 = tmpSeg.GetProduct(Account_Id, strLandlord, "", "", "", "", receiptCompany, "", "");
                                A4 = tmpSeg.GetCountry();
                                A5 = true;
                                tmpSeg.Seg_Inter = receiptCompany.Trim();
                                A7 = tmpSeg.GetSales();
                                if (A1 && A2 && A3 && A4 && A5 && A7)
                                {
                                    SetLineDis(clsReceipt, tmpSeg, "R", "CR", theAmount, strLineNo);
                                }
                                // (DR)Parent Company, C/A with Receipt Company, Account id=401
                                Account_Id = "401";
                                tmpSeg.GetCompany();
                                A1 = tmpSeg.GetAccount(Account_Id, strLandlord, strProperty, "", "", "", receiptCompany, "", "");
                                A2 = tmpSeg.GetCostCenter(Account_Id, strLandlord, strProperty, "", "", "", receiptCompany, "");
                                A3 = tmpSeg.GetProduct(Account_Id, strLandlord, strProperty, "", "", "", receiptCompany, "", "");
                                A4 = tmpSeg.GetCountry();
                                A5 = true;
                                tmpSeg.Seg_Inter = receiptCompany.Trim();
                                A7 = tmpSeg.GetSales();
                                if (A1 && A2 && A3 && A4 && A5 && A7)
                                {
                                    SetLineDis(clsReceipt, tmpSeg, "R", "DR", theAmount, strLineNo);
                                }
                                // (CR)Parent Company, C/A with Landlord Company, Account id=403
                                Account_Id = "403";
                                tmpSeg.GetCompany();
                                A1 = tmpSeg.GetAccount(Account_Id, strLandlord, strProperty, "", "", "", receiptCompany, "", "");
                                A2 = tmpSeg.GetCostCenter(Account_Id, strLandlord, strProperty, "", "", "", receiptCompany, "");
                                A3 = tmpSeg.GetProduct(Account_Id, strLandlord, strProperty, "", "", "", receiptCompany, "", "");
                                A4 = tmpSeg.GetCountry();
                                A5 = true;
                                tmpSeg.Seg_Inter = GetInterCodeForLandlord(strLandlord.Trim());
                                A7 = tmpSeg.GetSales();
                                if (A1 && A2 && A3 && A4 && A5 && A7)
                                {
                                    SetLineDis(clsReceipt, tmpSeg, "R", "CR", theAmount, strLineNo);
                                }
                                #endregion
                            }
                            else if (billingCompany != receiptCompany && receiptCompany != strLandlord)
                            {
                                #region Receipt 2.1.4	Billing Company ≠ Receive Company ≠ Landlord Company
                                // Receipt Company, Dr(Bank), Account_Id=302
                                Account_Id = "302";
                                tmpSeg.Seg_Company = receiptCompany;
                                A1 = tmpSeg.GetAccount(Account_Id, "", "", "", "", "", receiptCompany, bankAccount, "");
                                A2 = tmpSeg.GetCostCenter(Account_Id, "", strProperty, "", "", "", receiptCompany, bankAccount);
                                A3 = tmpSeg.GetProduct(Account_Id, "", "", "", "", "", receiptCompany, bankAccount, "");
                                A4 = tmpSeg.GetCountry(Account_Id, "", "", "", receiptCompany, bankAccount, "");
                                // A5 = tmpSeg.GetInter(Account_Id, strLandlord, "", "", "");
                                A5 = true;
                                //tmpSeg.Seg_Inter = strLandlord.Trim();
                                tmpSeg.Seg_Inter = GetInterCodeForLandlord(strLandlord.Trim());
                                A7 = tmpSeg.GetSales(Account_Id, "", "", "", "", "", receiptCompany, bankAccount, "");
                                if (A1 && A2 && A3 && A4 && A5 && A7)
                                {
                                    SetLineDis(clsReceipt, tmpSeg, "R", "DR", theAmount, strLineNo);
                                }
                                // CR(C/A with Parent Com (With Lease)), Account_Id=305
                                Account_Id = "305";
                                tmpSeg.Seg_Company = receiptCompany;
                                A1 = tmpSeg.GetAccount(Account_Id, strLandlord, "", "", "", "", receiptCompany, "", "");
                                A2 = tmpSeg.GetCostCenter(Account_Id, strLandlord, "", "", "", "", receiptCompany, "");
                                A3 = tmpSeg.GetProduct(Account_Id, strLandlord, "", "", "", "", receiptCompany, "", "");
                                A4 = tmpSeg.GetCountry();
                                // A5 = tmpSeg.GetInter(Account_Id, strLandlord, "", receiptCompany, "");
                                A5 = true;
                                tmpSeg.Seg_Inter = GetParentCom(receiptCompany);
                                A7 = tmpSeg.GetSales();
                                if (A1 && A2 && A3 && A4 && A5 && A7)
                                {
                                    SetLineDis(clsReceipt, tmpSeg, "R", "CR", theAmount, strLineNo);
                                }
                                // landlord Company, Dr(C/A with Parent Com (With Lease)), Account_Id=105
                                Account_Id = "105";
                                tmpSeg.Seg_Company = strLandlord;
                                A1 = tmpSeg.GetAccount(Account_Id, strLandlord, "", "", "", "", receiptCompany, "", "");
                                A2 = tmpSeg.GetCostCenter(Account_Id, strLandlord, "", "", "", "", receiptCompany, "");
                                A3 = tmpSeg.GetProduct(Account_Id, strLandlord, "", "", "", "", receiptCompany, "", "");
                                A4 = tmpSeg.GetCountry();
                                // A5 = tmpSeg.GetInter(Account_Id, strLandlord, "", receiptCompany, "");
                                A5 = true;
                                tmpSeg.Seg_Inter = GetParentCom(strLandlord);
                                A7 = tmpSeg.GetSales();
                                if (A1 && A2 && A3 && A4 && A5 && A7)
                                {
                                    SetLineDis(clsReceipt, tmpSeg, "R", "DR", theAmount, strLineNo);
                                }
                                // Landlord Company, CR(Debtors),Account_Id=101
                                Account_Id = "101";
                                tmpSeg.Seg_Company = strLandlord;
                                A1 = tmpSeg.GetAccount(Account_Id, strLandlord, strProperty, strChargeCode, "", "", "", "", "", strInternalGroup);  //Pong COA
                                A2 = tmpSeg.GetCostCenter(Account_Id, strLandlord, strProperty, strChargeCode, "", "", "", "");
                                A3 = tmpSeg.GetProduct(Account_Id, strLandlord, strProperty, strChargeCode, "", "", "", "", "");
                                A4 = tmpSeg.GetCountry();
                                //03/07/2015 10:46 RE: PropertyOne - major enhancement邮件要求
                                //A5 = tmpSeg.GetInter();
                                if (Check_Internal(strCustomer))
                                {
                                    tmpSeg.Seg_Inter = strCustomer.Substring(1, 4);
                                    A5 = true;
                                }
                                else
                                {
                                    A5 = tmpSeg.GetInter();
                                }
                                A7 = tmpSeg.GetSales();
                                if (A1 && A2 && A3 && A4 && A5 && A7)
                                {
                                    SetLineDis(clsReceipt, tmpSeg, "R", "CR", theAmount, strLineNo);
                                }
                                // Parent Company, Dr (C/A with Receipt Com(With Lease)) Account_ID=401
                                string strParent = GetParentCom(strLandlord);
                                Account_Id = "401";
                                A6 = tmpSeg.GetCompany();
                                A1 = tmpSeg.GetAccount(Account_Id, strLandlord, strProperty, "", "", "", receiptCompany, "", "");
                                A2 = tmpSeg.GetCostCenter(Account_Id, strLandlord, strProperty, "", "", "", receiptCompany, "");
                                A3 = tmpSeg.GetProduct(Account_Id, strLandlord, strProperty, "", "", "", receiptCompany, "", "");
                                A4 = tmpSeg.GetCountry();
                                // A5 = tmpSeg.GetInter(Account_Id, "", "", receiptCompany, "");
                                A5 = true;
                                tmpSeg.Seg_Inter = receiptCompany.Trim();
                                A7 = tmpSeg.GetSales();
                                if (A1 && A2 && A3 && A4 && A5 && A6 && A7)
                                {
                                    SetLineDis(clsReceipt, tmpSeg, "R", "DR", theAmount, strLineNo);
                                }
                                // Parent Company, CR(C/A with landlord), Account_ID=403
                                Account_Id = "403";
                                A6 = tmpSeg.GetCompany();
                                A1 = tmpSeg.GetAccount(Account_Id, strLandlord, strProperty, "", "", "", receiptCompany, "", "");
                                A2 = tmpSeg.GetCostCenter(Account_Id, strLandlord, strProperty, "", "", "", receiptCompany, "");
                                A3 = tmpSeg.GetProduct(Account_Id, strLandlord, strProperty, "", "", "", receiptCompany, "", "");
                                A4 = tmpSeg.GetCountry();
                                // A5 = tmpSeg.GetInter(Account_Id, strLandlord, "", "", "");
                                A5 = true;
                                //tmpSeg.Seg_Inter = strLandlord.Trim();
                                tmpSeg.Seg_Inter = GetInterCodeForLandlord(strLandlord.Trim());
                                A7 = tmpSeg.GetSales();
                                if (A1 && A2 && A3 && A4 && A5 && A6 && A7)
                                {
                                    SetLineDis(clsReceipt, tmpSeg, "R", "CR", theAmount, strLineNo);
                                }
                                #endregion
                            }
                        }
                        #endregion
                    }
                    else
                    { 
                        #region non-leased invoice offset coa mapping
                        //blank
                        #endregion
                    }
                }
                #endregion
            }
            else if (true)
            {
                // 如果Receipt有冲消,而且使用了Prepay
                //blank
            }
            result = this.LineDis;
        }
        catch(Exception err)
        {
            throw err;
        }

        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="companyCode"></param>
    /// <param name="trans"></param>
    /// <returns></returns>
    private string GetParentCom(string companyCode)
    {
        string result = "";
        string strSql = "Select Parent_Code from Ad_Company Where Upper(Trim(Company_Code)) = Upper('" + companyCode.Trim() + "') and Upper(Trim(Active))='A'";
        result = Common.GetSingleValue(strSql, this._db);
        return result;
    }

    /// <summary>
    /// 如果HWLG=N,那么Landlord Company是Outsite Company,返回0000,
    /// 否则返回Landlord Company
    /// </summary>
    /// <param name="strLandlord"></param>
    /// <param name="trans"></param>
    /// <returns></returns>
    private string GetInterCodeForLandlord(string strLandlord)
    {
        string strSQL = "select COUNT(*) from AD_COMPANY WHERE TRIM(COMPANY_CODE)='" + strLandlord + "' AND TRIM(HWLG)='N'";
        int counter = int.Parse(Common.GetSingleValue(strSQL, this._db));        
        if (counter > 0)
        {
            return "0000";
        }
        else
        {
            return strLandlord;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="propertycode"></param>
    /// <param name="trans"></param>
    /// <returns></returns>
    private bool Check_Wholly_Owned_Property(string propertycode)
    {
        bool result = true;
        string sqlstr = "select 1 from pd_property_landlord a,ad_company b where b.active='A' and trim(a.landlord_code)=trim(b.company_code) and nvl(b.is_wholly_owned,'N')='N' and trim(a.property_code)='" + propertycode.Trim().ToUpper() + "' ";
        DataTable dt = this._db.ExecuteDT(sqlstr);
        if (dt != null && dt.Rows.Count > 0)
        {
            result = false;
        }
        return result;
    }

    private bool Check_Group(string customerNumber)  //pong COA
    {
        bool isGroup = false;
        String sqlstr = "Select * From AD_COMPANY ";
        sqlstr += " Where UPPER(TRIM(COMPANY_CODE))=UPPER(TRIM('" + customerNumber.Trim().Substring(1) + "')) ";
        sqlstr += " and (CKP='Y' or HPG='Y' or HOTEL='Y') ";
        DataTable dt = this._db.ExecuteDT(sqlstr);
        if (dt != null && dt.Rows.Count > 0)
        {
            isGroup = true;
        }
        return isGroup;
    }

    private bool hasApplied(string receiptNumber)
    {
        string strSQL = "Select Count(*) From ar_application_line ";
        strSQL += " Where receipt_number='" + receiptNumber + "' ";
        strSQL += " And invoice_number is not null And charge_code<>'PREPAY' ";
        string strCount = Common.GetSingleValue(strSQL,this._db);
        if (strCount == "0")
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// 此Receipt冲消时是否使用了Prepay
    /// </summary>
    /// <param name="receiptNumber"></param>
    /// <param name="trans"></param>
    /// <returns></returns>
    private bool hasAppliedPrepay(string receiptNumber)
    {
        string strSQL = "Select count(*) From ar_application_line ";
        strSQL += " Where receipt_number<> From_receipt_number ";
        strSQL += " And charge_code='PREPAY' ";
        strSQL += " And receipt_number='" + receiptNumber + "'";
        string strCount = Common.GetSingleValue(strSQL, this._db);
        if (strCount == "0")
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private bool SetLineDis(M_Receipt rptmod, Segment clsSeg, string strTransType, String strDrCr, double dblAmount, string strLineNo)
    {
        bool result = false;
        try
        {
            if (dblAmount == 0)
            {
                return true;
            }
            LineDistribution tmpLineDis = new LineDistribution();
            tmpLineDis.Account = clsSeg.Seg_Account.Trim().ToUpper();
            tmpLineDis.Amount = dblAmount;
            tmpLineDis.Company = clsSeg.Seg_Company.Trim().ToUpper();
            tmpLineDis.Cost = clsSeg.Seg_Cost.Trim().ToUpper();
            tmpLineDis.Country = clsSeg.Seg_Country.Trim().ToUpper();
            tmpLineDis.Create_By = "";
            tmpLineDis.Creation_Date = "";
            tmpLineDis.Currency = rptmod.CURRENCY;
            tmpLineDis.Debit_Credit = strDrCr.Trim().ToUpper();
            tmpLineDis.Exchange_Rate = rptmod.RATE;
            tmpLineDis.Inter = clsSeg.Seg_Inter.Trim().ToUpper();
            tmpLineDis.Journal_Line_Number = "";
            tmpLineDis.Last_Updated_By = "";
            tmpLineDis.Last_Updated_Date = "";
            tmpLineDis.Line_No = strLineNo.Trim().ToUpper();
            tmpLineDis.Post_Status = "U";
            tmpLineDis.Product = clsSeg.Seg_Product.Trim().ToUpper();
            tmpLineDis.Project = clsSeg.Seg_Project.Trim().ToUpper();
            tmpLineDis.Remark = "";
            tmpLineDis.Sales = clsSeg.Seg_Sales.Trim().ToUpper();
            tmpLineDis.Spare = clsSeg.Seg_Spare.Trim().ToUpper();
            tmpLineDis.Transaction_No = rptmod.RECEIPT_NUMBER.Trim().ToUpper();
            tmpLineDis.Transaction_Type = strTransType.Trim().ToUpper();
            LineDis.Add(tmpLineDis);
            result = true;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return result;
    }

    private bool Check_Internal(string customerNumber)
    {
        bool isInternal = false;
        //
        if (customerNumber.Trim().Substring(0, 1).ToUpper() == "I")
        {
            isInternal = true;
        }
        if (isInternal)
        {
            String sqlstr = "Select Upper(Trim(HWLG)) HWLG From AD_COMPANY ";
            sqlstr += " Where UPPER(TRIM(COMPANY_CODE))=UPPER(TRIM('" + customerNumber.Trim().Substring(1) + "'))";
            DataTable dt = this._db.ExecuteDT(sqlstr);
            if (dt != null && dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["HWLG"].ToString() != "Y")
                {
                    isInternal = false;
                }
            }
        }
        //
        return isInternal;
    }
}