using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for Segment
/// </summary>
public class Segment
{
    private String _company;
    private String _account;
    private String _cost_center;
    private String _product_code;
    private String _country_code;
    private String _inter_code;
    private String _sales;
    private String _spare;
    private String _project;
    private String _seg_type;
    
    public String Seg_Company
    {
        get { return _company; }
        set { _company = value; }
    }
    public String Seg_Account
    {
        get { return _account; }
        set { _account = value; }
    }
    public String Seg_Cost
    {
        get { return _cost_center; }
        set { _cost_center = value; }
    }
    public String Seg_Product
    {
        get { return _product_code; }
        set { _product_code = value; }
    }
    public String Seg_Country
    {
        get { return _country_code; }
        set { _country_code = value; }
    }
    public String Seg_Inter
    {
        get { return _inter_code; }
        set { _inter_code = value; }
    }
    public String Seg_Sales
    {
        get { return _sales; }
        set { _sales = value; }
    }
    public String Seg_Spare
    {
        get { return _spare; }
        set { _spare = value; }
    }
    public String Seg_Project
    {
        get { return _project; }
        set { _project = value; }
    }
    public String Seg_Type
    {
        get { return _seg_type; }
        set { _seg_type = value; }
    }

    private DbHelper _db = null;

	public Segment(DbHelper db)
	{
        this._db = db; 
        this.GetProject();
        this.GetSales();
        this.GetSpare();
	}

    private void GetProject()
    {
        try
        {
            this._project = "";
            String sqlstr = "Select Default_Value from AD_COA_SEG Where Upper(TRIM(SEGMENT_CODE))=Upper('pjt')";
            DataTable dt = this._db.ExecuteDT(sqlstr);
            if (dt != null)
            {
                if (dt.Rows.Count > 1)
                {
                    throw new Exception("9907-Get default Project failed,please check COA Mapping setting.");
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    _project = dt.Rows[0]["Default_Value"].ToString();
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public bool GetSales()
    {
        bool result = false;
        try
        {
            //this._sales = "";
            String sqlstr = "Select Default_Value from AD_COA_SEG Where Upper(TRIM(SEGMENT_CODE))=Upper('sls')";
            DataTable dt = this._db.ExecuteDT(sqlstr);
            if (dt != null)
            {
                if (dt.Rows.Count > 1)
                {
                    //throw new Exception("9906-Get default Sales failed,please check COA Mapping setting.");
                    Exception e = new Exception("9906-Get default Sales failed,please check COA Mapping setting.");
                    result = false;
                    throw e;
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    _sales = dt.Rows[0]["Default_Value"].ToString();
                }
                result = true;
            }
        }
        catch (Exception ex)
        {
            result = false;
            throw ex;
        }
        return result;
    }

    private void GetSpare()
    {
        try
        {
            this._spare = "";
            String sqlstr = "Select Default_Value from AD_COA_SEG Where Upper(TRIM(SEGMENT_CODE))=Upper('spr')";
            DataTable dt = this._db.ExecuteDT(sqlstr);
            if (dt != null)
            {
                if (dt.Rows.Count > 1)
                {
                    throw new Exception("9908-Get default Spare failed,please check COA Mapping setting.");
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    _spare = dt.Rows[0]["Default_Value"].ToString();
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public bool GetAccount(string accid, string landlord, string property, string charge_code, string billing, string receipt_type, string receipt, string bank, string action)
    {//pong COA  default pass No (not internal group)
        return GetAccount(accid, landlord, property, charge_code, billing, receipt_type, receipt, bank, action, "N");  //pong COA 

    }

    public bool GetAccount(string accid, string landlord, string property, string charge_code, string billing, string receipt_type, string receipt, string bank, string action, string isGroup)  //pong COA 
    {
        bool result = false;
        string ckResult = "";
        DataTable dt = new DataTable();
        ArrayList sqlstr = new ArrayList();
        int i = 0;
        try
        {
            // 101, landlord company, Debtor, Landlord Company + Property + Charge Code 
            // 102, landlord company, Income, Landlord Company + Property + Charge Code
            // 106, landlord company, Deposit, Landlord Company + Property + Charge Code
            // 107, landlord company, Deposit refund, Landlord Company + Property + Charge Code
            if (accid == "101" || accid == "102" || accid == "106" || accid == "107")
            {
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "') AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')" + " and (Upper(Trim(INTERNAL))=Upper('" + isGroup.Trim() + "')) ");  //pong COA
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "') AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(INTERNAL) is null or Trim(INTERNAL)='')");  //pong COA
                //sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "') AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')"); //pong coa original

                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Upper(Trim(LANDLORD_COMPANY))='' or Trim(LANDLORD_COMPANY) is null) AND Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "') AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')" + " and (Upper(Trim(INTERNAL))=Upper('" + isGroup.Trim() + "')) ");  //pong COA
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Upper(Trim(LANDLORD_COMPANY))='' or Trim(LANDLORD_COMPANY) is null) AND Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "') AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(INTERNAL) is null or Trim(INTERNAL)='')");  //pong COA
                //sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Upper(Trim(LANDLORD_COMPANY))='' or Trim(LANDLORD_COMPANY) is null) AND Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "') AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");  //pong coa original

                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND (Trim(PROPERTY)='' or Trim(PROPERTY) is null) AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')" + " and (Upper(Trim(INTERNAL))=Upper('" + isGroup.Trim() + "')) ");  //pong COA
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND (Trim(PROPERTY)='' or Trim(PROPERTY) is null) AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(INTERNAL) is null or Trim(INTERNAL)='')");  //pong COA
                //sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND (Trim(PROPERTY)='' or Trim(PROPERTY) is null) AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");  //pong coa original

                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) AND (Trim(PROPERTY)='' or Trim(PROPERTY) is null)   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')" + " and (Upper(Trim(INTERNAL))=Upper('" + isGroup.Trim() + "')) ");  //pong COA
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) AND (Trim(PROPERTY)='' or Trim(PROPERTY) is null)   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(INTERNAL) is null or Trim(INTERNAL)='')");  //pong COA
                //sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) AND (Trim(PROPERTY)='' or Trim(PROPERTY) is null)   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");// pong COA original

                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "') And (Upper(Trim(CHARGE_CODE))='' or Trim(CHARGE_CODE) is null)   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')" + " and (Upper(Trim(INTERNAL))=Upper('" + isGroup.Trim() + "')) ");  //pong COA
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "') And (Upper(Trim(CHARGE_CODE))='' or Trim(CHARGE_CODE) is null)   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(INTERNAL) is null or Trim(INTERNAL)='')"); //pong coa
                //sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "') And (Upper(Trim(CHARGE_CODE))='' or Trim(CHARGE_CODE) is null)   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')"); //pong coa original

                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord + "') AND (Trim(PROPERTY)='' or Trim(PROPERTY) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')" + " and (Upper(Trim(INTERNAL))=Upper('" + isGroup.Trim() + "')) ");  //pong COA
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord + "') AND (Trim(PROPERTY)='' or Trim(PROPERTY) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='') And (Trim(INTERNAL) is null or Trim(INTERNAL)='')"); //pong coa
                //sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord + "') AND (Trim(PROPERTY)='' or Trim(PROPERTY) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')"); //pong coa original

                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null)   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')" + " and (Upper(Trim(INTERNAL))=Upper('" + isGroup.Trim() + "')) ");  //pong COA
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null)   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='') And (Trim(INTERNAL) is null or Trim(INTERNAL)='')");  //pong COA
                //sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null)   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");  //pong COA original

                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(PROPERTY)='' or Trim(PROPERTY) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null)   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')" + " and (Upper(Trim(INTERNAL))=Upper('" + isGroup.Trim() + "')) ");  //pong COA
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(PROPERTY)='' or Trim(PROPERTY) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null)   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(INTERNAL) is null or Trim(INTERNAL)='')");  //pong COA
                //sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(PROPERTY)='' or Trim(PROPERTY) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null)   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");  //pong coa original
            }
            // 103, landlord company, Bank(with lease), Receipt Company + Bank
            // 110, landlord company, Suspense(with lease), Receipt Company + Bank
            // 302, Receipt Company, Bank(With Lease), Receipt Company + Bank
            // 303, Receipt Company, Bank (Without Lease), Receipt Company + bank
            // 312, Receipt Company, Suspense(With Lease), Receipt Company + Bank
            // 313, Receipt Company, Suspense (Without Lease), Receipt Company + bank
            // 304, Receipt Company, Bank (Unidentified), Receipt Company + Bank
            if (accid == "103" || accid == "110" || accid == "302" || accid == "303" || accid == "312" || accid == "313" || accid == "304")
            {
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "')  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "')  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "')  AND (Trim(Bank)='' or Trim(Bank) is null)  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null)  AND (Trim(Bank)='' or Trim(Bank) is null)  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
            }
            // 104, landlord company, C/A with Receipt Com(With Lease), Landlord Company + Receipt Company
            // 105, landlord company, C/A with Parent Com (With Lease), Landlord Company + Receipt Company
            // 305, Receipt Company, C/A with Parent Com (With Lease), Receipt Company + Landlord Company
            // 309, Receipt Company, C/A with landlord, Receipt Company + Landlord Company 
            if (accid == "104" || accid == "105" || accid == "305" || accid == "309")
            {
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "')  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='') And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null)  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='') And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "')  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='') And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null)  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='') And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
            }
            // 108, landlord company, Bad debt/Other Income, "Action" field
            if (accid == "108")
            {

                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(Act))=Upper('" + action.Trim() + "')   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')" + " and (Upper(Trim(INTERNAL))=Upper('" + isGroup.Trim() + "')) ");  //pong COA
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(Act))=Upper('" + action.Trim() + "')   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')  And (Trim(INTERNAL) is null or Trim(INTERNAL)='')");  //pong COA
                //sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(Act))=Upper('" + action.Trim() + "')   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");  //pong coa original
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(Act)='' or Trim(Act) is null)   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')" + " and (Upper(Trim(INTERNAL))=Upper('" + isGroup.Trim() + "')) ");  //pong COA
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(Act)='' or Trim(Act) is null)   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(INTERNAL) is null or Trim(INTERNAL)='')");  //pong COA
                //sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(Act)='' or Trim(Act) is null)   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");//pong coa original
            }

            // 109, landlord company, Unidentified Receipt, Receipt Company + Receipt Type
            // 205, Billing Company, Unidentified Receipt, Receipt Company + Receipt Type
            // 207, Billing Company, Suspense, Receipt Company + Receipt Type
            // 307, Receipt Company, Unidentified Receipt,Receipt Company + Receipt Type
            if (accid == "109" || accid == "205" || accid == "207" || accid == "307")
            {
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "')  And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "')  And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null)  And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null)  And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
            }
            // 201, Billing Company, Debtor, Billing Company + Charge Code
            // 202, Billing Company, Income, Billing Company + Charge Code
            // 301, Receipt Company, Debtor, Billing Company + Charge Code
            if (accid == "201" || accid == "202" || accid == "301")
            {
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')" + " AND (Upper(Trim(INTERNAL))=Upper('" + isGroup.Trim() + "')) ");  // pong coa
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(INTERNAL) is null or Trim(INTERNAL)='')");  //pong coa
                //sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");  //pong coa original

                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')" + " AND (Upper(Trim(INTERNAL))=Upper('" + isGroup.Trim() + "')) ");  // pong coa
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(INTERNAL) is null or Trim(INTERNAL)='')");  //pong coa                    
                //sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");  //pong coa original

                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')" + " AND (Upper(Trim(INTERNAL))=Upper('" + isGroup.Trim() + "')) ");  // pong coa
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(INTERNAL) is null or Trim(INTERNAL)='')");  //PONG COA
                //sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");  //PONG COA ORIGINAL
            }

            // 203, Billing Company, C/A with Receipt Com(Without Lease), Billing Company + Receipt Company
            // 204, Billing Company, C/A with Parent Com (Without Lease), Billing Company + Receipt Company
            // 306, Receipt Company, C/A with Parent Com (Without Lease),Receipt Company + Billing Company
            // 310, Receipt Company, C/A with Billing Company, Receipt Company + Billing Company
            // 402, Parent Company, C/A with Receipt Com(Without Lease), Billing Company + Receipt Company
            // 404, Parent Company, C/A with Billing Company, Billing Company + Receipt Company
            if (accid == "203" || accid == "204" || accid == "306" || accid == "310" || accid == "402" || accid == "404")
            {
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
            }
            // 206, Billing Company, Inter-co, Billing Company + Bank + Receipt type + Charge Code
            if (accid == "206")
            {
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) And Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) And (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) And (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");

                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) And Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(Bank)='' or Trim(Bank) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) And Upper(Trim(Bank))=Upper('" + bank.Trim() + "') AND (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");

                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') And (Trim(Bank)='' or Trim(Bank) is null) AND (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) And Upper(Trim(Bank))=Upper('" + bank.Trim() + "') AND (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) And (Trim(Bank)='' or Trim(Bank) is null) AND Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) And (Trim(Bank)='' or Trim(Bank) is null) AND (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
            }
            // 308, Receipt Company, Inter-co, Receiving Company  + Bank + Receipt type + Charge Code
            if (accid == "308")
            {
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");

                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");

                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
            }
            // 311, Receipt Company, C/A with customer, Receipt Company + Billing Company + Bank + Receipt type + Charge Code
            if (accid == "311")
            {
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') and Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) and Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) and (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) and (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");

                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') and Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') and Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') and (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) and Upper(Trim(Bank))=Upper('" + bank.Trim() + "') and Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");

                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') And (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");

                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') And (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') And (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");

                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
            }
            // 401, Parent Company, C/A with Receipt Com(With Lease), Landlord Company + Receipt Co + Property
            // 403, Parent Company, C/A with landlord, Landlord Company + Receipt Co + Property
            if (accid == "401" || accid == "403")
            {
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "')  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) AND Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "') AND Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "')  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND (Trim(PROPERTY)='' or Trim(PROPERTY) is null) AND Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "')  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) AND (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "')  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");

                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "')  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And (Trim(PROPERTY)='' or Trim(PROPERTY) is null)  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) AND Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And (Trim(PROPERTY)='' or Trim(PROPERTY) is null)  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) AND (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And (Trim(PROPERTY)='' or Trim(PROPERTY) is null)  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
            }
            //501,Rent-Free Income,Dr(Rent-Free Allowance),Landlord Company+Charge Code
            //502,Rent-Free Income,Cr(Rental Income),Landlord Company+Charge Code
            //503,Rent-Free Amortization,Dr(Rent-Free Amortization),Landlord Company+Charge Code
            //504,Rent-Free Amortization,Cr(Accumulated Amortization),Landlord Company+Charge Code
            //505,Rent Incentive Amortization,Dr(Rent Incentive Amortization),Landlord Company+Charge Code
            //506,Rent Incentive Amortization,Cr(Rent Incentive Allowance),Landlord Company+Charge Code
            //601,Step-Rent Income,Dr(Step Rent),Landlord Company+Charge Code
            //602,Step-Rent Income,Cr(Rental Income),Landlord Company+Charge Code
            //603,Step-Rent Amortization,Dr(Step Rent Amortization),Landlord Company+Charge Code
            //604,Step-Rent Amortization,Cr(Acc Amort-Step Rent),Landlord Company+Charge Code
            if (accid == "501" || accid == "502" || accid == "503" || accid == "504" || accid == "505" || accid == "506" || accid == "601" || accid == "602" || accid == "603" || accid == "604")
            {
                //Landlord Company + Charge Code
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND (PROPERTY is Null Or Trim(PROPERTY)='') AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')" + " and (Upper(Trim(INTERNAL))=Upper('" + isGroup.Trim() + "')) ");  //pong COA
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND (PROPERTY is Null Or Trim(PROPERTY)='') AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(INTERNAL) is null or Trim(INTERNAL)='')");  // PONG COA
                //sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND (PROPERTY is Null Or Trim(PROPERTY)='') AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')"); //pong coa original 
                //Only Landlord Company
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND (PROPERTY is Null Or Trim(PROPERTY)='') AND (CHARGE_CODE is Null Or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')" + " and (Upper(Trim(INTERNAL))=Upper('" + isGroup.Trim() + "')) ");  //pong COA
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND (PROPERTY is Null Or Trim(PROPERTY)='') AND (CHARGE_CODE is Null Or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(INTERNAL) is null or Trim(INTERNAL)='')");  // PONG COA
                //sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND (PROPERTY is Null Or Trim(PROPERTY)='') AND (CHARGE_CODE is Null Or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");  //pong coa original
                //Only Charge Code
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (LANDLORD_COMPANY is Null Or Trim(LANDLORD_COMPANY)='') AND (PROPERTY is Null Or Trim(PROPERTY)='') AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='') " + " and (Upper(Trim(INTERNAL))=Upper('" + isGroup.Trim() + "')) ");  //pong COA
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (LANDLORD_COMPANY is Null Or Trim(LANDLORD_COMPANY)='') AND (PROPERTY is Null Or Trim(PROPERTY)='') AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(INTERNAL) is null or Trim(INTERNAL)='')");  // PONG COA
                //sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (LANDLORD_COMPANY is Null Or Trim(LANDLORD_COMPANY)='') AND (PROPERTY is Null Or Trim(PROPERTY)='') AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')"); //PONG COA ORIGINAL
                //All null
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (LANDLORD_COMPANY is Null Or Trim(LANDLORD_COMPANY)='') AND (PROPERTY is Null Or Trim(PROPERTY)='') AND (CHARGE_CODE is Null Or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')" + " and (Upper(Trim(INTERNAL))=Upper('" + isGroup.Trim() + "')) ");  //pong COA
                sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (LANDLORD_COMPANY is Null Or Trim(LANDLORD_COMPANY)='') AND (PROPERTY is Null Or Trim(PROPERTY)='') AND (CHARGE_CODE is Null Or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(INTERNAL) is null or Trim(INTERNAL)='')");  //PONG COA
                //sqlstr.Add("Select Seg_Value from COA_Naturalac Where ACCOUNT_ID=To_Number('" + accid + "') and (LANDLORD_COMPANY is Null Or Trim(LANDLORD_COMPANY)='') AND (PROPERTY is Null Or Trim(PROPERTY)='') AND (CHARGE_CODE is Null Or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");  //PONG COA ORIGINAL
            }
            //
            while (ckResult.Length < 1 && i < sqlstr.Count)
            {
                for (i = 0; i < sqlstr.Count; i++)
                {
                    if (ckResult == "")
                    {
                        dt = this._db.ExecuteDT(sqlstr[i].ToString());
                        ckResult = CheckDT(dt);
                    }
                }

            }
            if (ckResult.Length > 0)
            {
                _account = ckResult;
                result = true;
            }
            else
            {
                Exception e = new Exception("9901- Get natural account failed,please check COA Mapping setting. Parameter is : AccountId-" + accid + ", landlord-" + landlord + ", property-" + property + ", charge_code-" + charge_code + ", billing-" + billing + ", receipt_type-" + receipt_type + ", receipt_Company-" + receipt + ", bank-" + bank + ", action-" + action);
                result = false;
                throw e;
            }
        }
        catch (Exception ex)
        {
            result = false;
            throw ex;
        }
        return result;
    }
   
    /// <summary>
    /// 由于Company Segment直接就是Company Code，因此没有必要写单独的方法获得Company Segment的值，所以这两个方法不需要
    /// </summary>
    /// <returns></returns>
    public bool GetCompany()
    {
        bool result = false;
        try
        {
            String sqlstr = "Select Default_Value from AD_COA_Seg Where UPPER(TRIM(SEGMENT_CODE))=UPPER('cpy')";
            DataTable dt = this._db.ExecuteDT(sqlstr);
            if (dt != null)
            {
                if (dt.Rows.Count > 1)
                {
                    Exception e = new Exception("9900 - Get default Company failed, please check COA Mapping setting.");
                    // e.Source = "class Segment, method GetCompany()";
                    // e.Message = "9900";
                    result = false;
                    throw e;
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    _company = dt.Rows[0]["Default_Value"].ToString();
                }
                result = true;
            }
        }
        catch (Exception ex)
        {
            result = false;
            throw ex;
        }
        return result;
    }

    /// <summary>
    /// Get Cost/Profit Center Default Value
    /// </summary>
    /// <returns></returns>
    public bool GetCostCenter()
    {
        bool result = false;
        try
        {
            String sqlstr = "Select Default_Value from AD_COA_Seg Where Upper(TRIM(SEGMENT_CODE))=Upper('cst')";
            DataTable dt = this._db.ExecuteDT(sqlstr);
            if (dt != null)
            {
                if (dt.Rows.Count > 1)
                {
                    Exception e = new Exception("9902-Get default Cost Center failed,please check COA Mapping setting.");
                    // e.Source = "class Segment, method GetCostCenter()";
                    // e.Message = "9902";
                    result = false;
                    throw e;
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    _cost_center = dt.Rows[0]["Default_Value"].ToString();
                }
                result = true;
            }
        }
        catch (Exception ex)
        {
            result = false;
            throw ex;
        }
        return result;
    }

    public bool GetCostCenter(string accid, string landlord, string property, string charge_code, string billing, string receipt_type, string receipt, string bank)
    {
        bool result = false;
        string ckResult = "";
        DataTable dt = new DataTable();
        ArrayList sqlstr = new ArrayList();
        int i = 0;
        try
        {
            // 101, Landlord Company, Debtor, Landlord Company + Property + Charge Code
            // 102, Landlord Company, Income, Landlord Company + Property + Charge Code
            // 106, Landlord Company, Deposit, Landlord Company + Property + Charge Code
            // 107, Landlord Company, Deposit refund, Landlord Company + Property + Charge Code
            if (accid == "101" || accid == "102" || accid == "106" || accid == "107")
            {
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "') AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) AND Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "') AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND (Trim(PROPERTY)='' or Trim(PROPERTY) is null) AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) AND (Trim(PROPERTY)='' or Trim(PROPERTY) is null)   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')");

                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND (Trim(PROPERTY)='' or Trim(PROPERTY) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null)   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(PROPERTY)='' or Trim(PROPERTY) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null)   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')");
            }
            // 103, Landlord Company, Bank(with lease), Receipt Company + Property + Bank
            // 110, Landlord Company, Suspense(with lease), Receipt Company + Property + Bank
            // 302, Receipt Company, Bank(With Lease), Receipt Company + Property + Bank
            // 312, Receipt Company, Suspense(With Lease), Receipt Company + Property + Bank
            if (accid == "103" || accid == "110" || accid == "302" || accid == "312")
            {
                //Receipt Company + Property + Bank
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "')   And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='')");
                //Property + Bank
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "')   And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='')");
                //Receipt Company + Bank
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND (Trim(PROPERTY)='' or Trim(PROPERTY) is null) AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "')   And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='')");
                //Only Bank
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND (Trim(PROPERTY)='' or Trim(PROPERTY) is null) AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "')   And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='')");
                //Receipt Company + Property
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "') And (Trim(Bank)='' or Trim(Bank) is null)   And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='')");
                //Only Receipt Company
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And (Trim(PROPERTY)='' or Trim(PROPERTY) is null) AND (Trim(Bank)='' or Trim(Bank) is null)   And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='')");
                //Only Property
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "') AND (Trim(Bank)='' or Trim(Bank) is null)   And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='')");
                //All null
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND (Trim(PROPERTY)='' or Trim(PROPERTY) is null) AND (Trim(Bank)='' or Trim(Bank) is null)   And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='')");
            }
            // 104, Landlord Company, C/A with Receipt Com(With Lease), Landlord Company + Receipt Company
            // 105, Landlord Company, C/A with Parent Com (With Lease), Landlord Company + Receipt Company
            // 305, Receipt Company, C/A with Parent Com (With Lease), Receipt Company + Landlord Company
            // 309, Receipt Company, C/A with landlord, Receipt Company + Landlord Company 
            if (accid == "104" || accid == "105" || accid == "305" || accid == "309")
            {
                //Landlord Company + Receipt Company
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')");
                //Only Landlord Company
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null)  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')");
                //Only Receipt Company
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')");
                //All null
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null)  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')");
            }
            // 108, Landlord Company, Bad debt/Other Income, Landlord Company + Property
            if (accid == "108")
            {
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "')   And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) AND Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "')   And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND (Trim(PROPERTY)='' or Trim(PROPERTY) is null)   And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) AND (Trim(PROPERTY)='' or Trim(PROPERTY) is null)   And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')");
            }
            // 109, Landlord Company, Unidentified Receipt, Receipt Company + Receipt Type
            // 205, Billing Company, Unidentified Receipt, Receipt Company + Receipt Type
            // 207, Billing Company, Suspense, Receipt Company + Receipt Type
            // 307, Receipt Company, Unidentified Receipt, Receipt Company + Receipt Type
            if (accid == "109" || accid == "205" || accid == "207" || accid == "307")
            {
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(BANK) is null or Trim(BANK)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(BANK) is null or Trim(BANK)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(BANK) is null or Trim(BANK)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(BANK) is null or Trim(BANK)='')");
            }
            // 201, Billing Company, Debtor,  if user define, use defined cost center.  If not define, Billing Co + Charge Code
            // 202, Billing Company, Income, if user define, use defined cost centre.  If not define, Billing Co + Charge Code
            // 301, Receipt Company, Debtor, Billing Company + Charge Code
            if (accid == "201" || accid == "202" || accid == "301")
            {
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')");
            }
            // 203, Billing Company, C/A with Receipt Com(Without Lease), Billing Company + Receipt Company
            // 204, Billing Company, C/A with Parent Com (Without Lease), Billing Company + Receipt Company
            // 306, Receipt Company, C/A with Parent Com (Without Lease), Receipt Company + Billing Company
            // 310, Receipt Company, C/A with Billing Company, Receipt Company + Billing Company
            // 402, Parent Company, C/A with Receipt Com(Without Lease), Billing Company + Receipt Company
            // 404, Parent Company, C/A with Billing Company, Billing Company + Receipt Company
            if (accid == "203" || accid == "204" || accid == "306" || accid == "310" || accid == "402" || accid == "404")
            {
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null)  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null)  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')");
            }
            // 206, Billing Company, Inter-co, Billing Company + Bank + Receipt type + Charge Code
            if (accid == "206")
            {
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) And Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) And (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) And (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And  Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");

                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) And Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(Bank)='' or Trim(Bank) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) And Upper(Trim(Bank))=Upper('" + bank.Trim() + "') AND (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");

                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') And (Trim(Bank)='' or Trim(Bank) is null) AND (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) And Upper(Trim(Bank))=Upper('" + bank.Trim() + "') AND (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) And (Trim(Bank)='' or Trim(Bank) is null) AND Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) And (Trim(Bank)='' or Trim(Bank) is null) AND (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
            }
            // 303, Receipt Company, Bank (Without Lease), Receipt Company + bank
            // 313, Receipt Company, Suspense (Without Lease), Receipt Company + bank
            // 304, Receipt Company, Bank (Unidentified), Receipt Company + Bank
            if (accid == "303" || accid == "313" || accid == "304")
            {
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND (Trim(Bank)='' or Trim(Bank) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND (Trim(Bank)='' or Trim(Bank) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='')");
            }
            // 308, Receipt Company, Inter-co, Receiving Company  + Bank + Receipt type + Charge Code
            if (accid == "308")
            {
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");

                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");

                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
            }
            // 311, Receipt Company, C/A with customer, Receipt Company + Billing Company + Bank + Receipt type + Charge Code
            if (accid == "311")
            {
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') and Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) and Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) and (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) and (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");

                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') and Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') and Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') and (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) and Upper(Trim(Bank))=Upper('" + bank.Trim() + "') and Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");

                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') And (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");

                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') And (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') And (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");

                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
            }
            // 401, Parent Company, C/A with Receipt Com(With Lease), Landlord Company + Receipt Co + Property
            // 403, Parent Company, C/A with landlord, Landlord Company + Receipt Co + Property
            if (accid == "401" || accid == "403")
            {
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "')  And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) AND Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "') AND Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "')  And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND (Trim(PROPERTY)='' or Trim(PROPERTY) is null) AND Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "')  And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) AND (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "')  And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')");

                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "')  And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And (Trim(PROPERTY)='' or Trim(PROPERTY) is null)  And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) AND Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And (Trim(PROPERTY)='' or Trim(PROPERTY) is null)  And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')");
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) AND (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And (Trim(PROPERTY)='' or Trim(PROPERTY) is null)  And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')");
            }
            //501,Rent-Free Income,Dr(Rent-Free Allowance),Landlord Company+Property
            //502,Rent-Free Income,Cr(Rental Income),Landlord Company+Property
            //503,Rent-Free Amortization,Dr(Rent-Free Amortization),Landlord Company+Property
            //504,Rent-Free Amortization,Cr(Accumulated Amortization),Landlord Company+Property
            //505,Rent Incentive Amortization,Dr(Rent Incentive Amortization),Landlord Company+Property
            //506,Rent Incentive Amortization,Cr(Rent Incentive Allowance),Landlord Company+Property
            //601,Step-Rent Income,Dr(Step Rent),Landlord Company+Property
            //602,Step-Rent Income,Cr(Rental Income),Landlord Company+Property
            //603,Step-Rent Amortization,Dr(Step Rent Amortization),Landlord Company+Property
            //604,Step-Rent Amortization,Cr(Acc Amort-Step Rent),Landlord Company+Property
            if (accid == "501" || accid == "502" || accid == "503" || accid == "504" || accid == "505" || accid == "506" || accid == "601" || accid == "602" || accid == "603" || accid == "604")
            {
                //Landlord Company + Property
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "') AND (CHARGE_CODE is Null Or Trim(CHARGE_CODE)='')   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')");
                //Only Landlord Company
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND (PROPERTY is Null Or Trim(PROPERTY)='') AND (CHARGE_CODE is Null Or Trim(CHARGE_CODE)='')   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')");
                //Only Property
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (LANDLORD_COMPANY is Null Or Trim(LANDLORD_COMPANY)='') AND Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "') AND (CHARGE_CODE is Null Or Trim(CHARGE_CODE)='')   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')");
                //All null
                sqlstr.Add("Select Seg_Value from COA_Cost_Center Where ACCOUNT_ID=To_Number('" + accid + "') and (LANDLORD_COMPANY is Null Or Trim(LANDLORD_COMPANY)='') AND (PROPERTY is Null Or Trim(PROPERTY)='') AND (CHARGE_CODE is Null Or Trim(CHARGE_CODE)='')   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')");
            }
            //
            while (ckResult.Length < 1 && i < sqlstr.Count)
            {
                for (i = 0; i < sqlstr.Count; i++)
                {
                    if (ckResult == "")
                    {
                        dt = this._db.ExecuteDT(sqlstr[i].ToString());
                        ckResult = CheckDT(dt);
                    }

                }
            }
            if (ckResult.Length > 0)
            {
                _cost_center = ckResult;
                result = true;
            }
            else
            {
                Exception e = new Exception("9902 - Get cost center failed,please check COA Mapping setting. Parameter is : AccountId-" + accid + ", landlord-" + landlord + ", property-" + property + ", charge_code-" + charge_code + ", billing-" + billing + ", receipt_type-" + receipt_type + ", receipt_Company-" + receipt + ", bank-" + bank);
                result = false;
                throw e;
            }
        }
        catch (Exception ex)
        {
            result = false;
            throw ex;
        }
        return result;
    }

    public bool GetProduct(string accid, string landlord, string property, string charge_code, string billing, string receipt_type, string receipt, string bank, string action)
    {
        return GetProduct(accid, landlord, property, charge_code, billing, receipt_type, receipt, bank, action, "");
    }

    public bool GetProduct(string accid, string landlord, string property, string charge_code, string billing, string receipt_type, string receipt, string bank, string action, string ptype)
    {
        bool result = false;
        string ckResult = "";
        DataTable dt = new DataTable();
        ArrayList sqlstr = new ArrayList();
        int i = 0;
        try
        {
            // 101, Landlord Company, Debtor, Landlord Company + Property + Charge Code
            // 102, Landlord Company, Income, Landlord Company + Property + Charge Code
            // 106, Landlord Company, Deposit, Landlord Company + Property + Charge Code
            // 107, Landlord Company, Deposit refund, Landlord Company + Property + Charge Code
            if (accid == "101" || accid == "102" || accid == "106" || accid == "107")
            {
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "') AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) AND Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "') AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND (Trim(PROPERTY)='' or Trim(PROPERTY) is null) AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) AND (Trim(PROPERTY)='' or Trim(PROPERTY) is null) AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')" + " AND Upper(Trim(ptype))=Upper('" + ptype.Trim() + "') ");  //pong coa
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) AND (Trim(PROPERTY)='' or Trim(PROPERTY) is null) AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='') And (Trim(ptype) is null or Trim(ptype)='')");  //pong coa
                //sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) AND (Trim(PROPERTY)='' or Trim(PROPERTY) is null) AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");//pong coa original

                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "') AND (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND (Trim(PROPERTY)='' or Trim(PROPERTY) is null) AND (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) AND Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "') AND (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) AND (Trim(PROPERTY)='' or Trim(PROPERTY) is null) AND (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
            }

            // 103, Landlord Company, Bank(with lease), Receipt Company + Bank
            // 110, Landlord Company, Suspense(with lease), Receipt Company + Bank
            // 302, Receipt Company, Bank(With Lease), Receipt Company + Bank
            // 312, Receipt Company, Suspense(With Lease), Receipt Company + Bank
            // 303, Receipt Company, Bank (Without Lease), Receipt Company + bank
            // 313, Receipt Company, Suspense (Without Lease), Receipt Company + bank
            // 304, Receipt Company, Bank (Unidentified), Receipt Company + Bank
            if (accid == "103" || accid == "110" || accid == "302" || accid == "312" || accid == "303" || accid == "313" || accid == "304")
            {
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND (Trim(Bank)='' or Trim(Bank) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND (Trim(Bank)='' or Trim(Bank) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(ACT) is null or Trim(ACT)='')");
            }
            // 104, Landlord Company, C/A with Receipt Com(With Lease), Landlord Company + Receipt Company
            // 105, Landlord Company, C/A with Parent Com (With Lease), Landlord Company + Receipt Company
            // 305, Receipt Company, C/A with Parent Com (With Lease), Receipt Company + Landlord Company
            // 309, Receipt Company, C/A with landlord, Receipt Company + Landlord Company 
            if (accid == "104" || accid == "105" || accid == "305" || accid == "309")
            {
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null)  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null)  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
            }
            // 108, Landlord Company, Bad debt/Other Income, "Action" field
            if (accid == "108")
            {
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(Act))=Upper('" + action.Trim() + "')    And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')" + " AND Upper(Trim(ptype))=Upper('" + ptype.Trim() + "') ");  //pong coa
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(Act))=Upper('" + action.Trim() + "')    And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ptype) is null or Trim(ptype)='')");  //pong coa
                //sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(Act))=Upper('" + action.Trim() + "')    And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')"); //pong coa original
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(Act)='' or Trim(Act) is null)    And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')" + " AND Upper(Trim(ptype))=Upper('" + ptype.Trim() + "') ");  //pong coa
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(Act)='' or Trim(Act) is null)    And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ptype) is null or Trim(ptype)='')");  //pong coa
                //sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(Act)='' or Trim(Act) is null)    And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')");  //pong coa original
            }
            // 109, Landlord Company, Unidentified Receipt, Receipt Company + Receipt Type
            // 205, Billing Company, Unidentified Receipt, Receipt Company + Receipt Type
            // 207, Billing Company, Suspense, Receipt Company + Receipt Type
            // 307, Receipt Company, Unidentified Receipt, Receipt Company + Receipt Type
            if (accid == "109" || accid == "205" || accid == "207" || accid == "307")
            {
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
            }
            // 201, Billing Company, Debtor, Billing Company + Charge Code
            // 202, Billing Company, Income, Billing Company + Charge Code
            // 301, Receipt Company, Debtor, Billing Company + Charge Code
            if (accid == "201" || accid == "202" || accid == "301")
            {
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='') " + " AND Upper(Trim(ptype))=Upper('" + ptype.Trim() + "') ");  //pong coa
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='') And (Trim(ptype) is null or Trim(ptype)='')");  //pong coa
                //sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");// pong coa original
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
            }

            // 203, Billing Company, C/A with Receipt Com(Without Lease), Billing Company + Receipt Company
            // 204, Billing Company, C/A with Parent Com (Without Lease), Billing Company + Receipt Company
            // 306, Receipt Company, C/A with Parent Com (Without Lease), Receipt Company + Billing Company
            // 310, Receipt Company, C/A with Billing Company, Receipt Company + Billing Company
            // 402, Parent Company, C/A with Receipt Com(Without Lease), Billing Company + Receipt Company
            // 404, Parent Company, C/A with Billing Company, Billing Company + Receipt Company
            if (accid == "203" || accid == "204" || accid == "306" || accid == "310" || accid == "402" || accid == "404")
            {
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
            }


            // 206, Billing Company, Inter-co, Billing Company + Bank + Receipt type + Charge Code
            if (accid == "206")
            {
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')  And (Trim(ACT) is null or Trim(ACT)='')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')  And (Trim(ACT) is null or Trim(ACT)='')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')  And (Trim(ACT) is null or Trim(ACT)='')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')  And (Trim(ACT) is null or Trim(ACT)='')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)  And (Trim(ACT) is null or Trim(ACT)='')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')  And (Trim(ACT) is null or Trim(ACT)='')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') And (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')  And (Trim(ACT) is null or Trim(ACT)='')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");

                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)  And (Trim(ACT) is null or Trim(ACT)='')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)  And (Trim(ACT) is null or Trim(ACT)='')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') And (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)  And (Trim(ACT) is null or Trim(ACT)='')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')  And (Trim(ACT) is null or Trim(ACT)='')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')  And (Trim(ACT) is null or Trim(ACT)='')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");

                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)  And (Trim(ACT) is null or Trim(ACT)='')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)  And (Trim(ACT) is null or Trim(ACT)='')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)  And (Trim(ACT) is null or Trim(ACT)='')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)  And (Trim(ACT) is null or Trim(ACT)='')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
            }
            // 308, Receipt Company, Inter-co, Receiving Company  + Bank + Receipt type + Charge Code
            if (accid == "308")
            {
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(ACT) is null or Trim(ACT)='')");

                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(ACT) is null or Trim(ACT)='')");

                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(ACT) is null or Trim(ACT)='')");
            }
            // 311, Receipt Company, C/A with customer, Receipt Company + Billing Company + Bank + Receipt type + Charge Code
            if (accid == "311")
            {
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') and Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) and Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) and (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) and (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");

                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') and Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') and Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') and (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) and Upper(Trim(Bank))=Upper('" + bank.Trim() + "') and Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");

                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) and Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) and Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') And (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) and (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");

                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) and Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') And (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) and Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) and (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) and (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') And (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') And (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) and Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");

                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) and (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) and Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) and (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') and (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) and (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
            }
            // 401, Parent Company, C/A with Receipt Com(With Lease), Landlord Company + Receipt Company + Property
            // 403, Parent Company, C/A with landlord, Landlord Company + Receipt Company + Property
            if (accid == "401" || accid == "403")
            {
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "')  And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) AND Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "') AND Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "')  And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND (Trim(PROPERTY)='' or Trim(PROPERTY) is null) AND Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "')  And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) AND (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "')  And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");

                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "')  And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') And (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And (Trim(PROPERTY)='' or Trim(PROPERTY) is null)  And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) AND Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And (Trim(PROPERTY)='' or Trim(PROPERTY) is null)  And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) AND (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And (Trim(PROPERTY)='' or Trim(PROPERTY) is null)  And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
            }
            //501,Rent-Free Income,Dr(Rent-Free Allowance),Landlord Company+Charge Code
            //502,Rent-Free Income,Cr(Rental Income),Landlord Company+Charge Code
            //503,Rent-Free Amortization,Dr(Rent-Free Amortization),Landlord Company+Charge Code
            //504,Rent-Free Amortization,Cr(Accumulated Amortization),Landlord Company+Charge Code
            //505,Rent Incentive Amortization,Dr(Rent Incentive Amortization),Landlord Company+Charge Code
            //506,Rent Incentive Amortization,Cr(Rent Incentive Allowance),Landlord Company+Charge Code
            //601,Step-Rent Income,Dr(Step Rent),Landlord Company+Charge Code
            //602,Step-Rent Income,Cr(Rental Income),Landlord Company+Charge Code
            //603,Step-Rent Amortization,Dr(Step Rent Amortization),Landlord Company+Charge Code
            //604,Step-Rent Amortization,Cr(Acc Amort-Step Rent),Landlord Company+Charge Code
            if (accid == "501" || accid == "502" || accid == "503" || accid == "504" || accid == "505" || accid == "506" || accid == "601" || accid == "602" || accid == "603" || accid == "604")
            {
                //Landlord Company+Charge Code
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND (PROPERTY is Null Or Trim(PROPERTY)='') AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                //Only Landlord Company
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND (PROPERTY is Null Or Trim(PROPERTY)='') AND (CHARGE_CODE is Null Or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                //Only Charge Code
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (LANDLORD_COMPANY is Null Or Trim(LANDLORD_COMPANY)='') AND (PROPERTY is Null Or Trim(PROPERTY)='') AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')" + " AND Upper(Trim(ptype))=Upper('" + ptype.Trim() + "') ");  //pong coa
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (LANDLORD_COMPANY is Null Or Trim(LANDLORD_COMPANY)='') AND (PROPERTY is Null Or Trim(PROPERTY)='') AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='') And (Trim(ptype) is null or Trim(ptype)='')");  //pong coa
                //sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (LANDLORD_COMPANY is Null Or Trim(LANDLORD_COMPANY)='') AND (PROPERTY is Null Or Trim(PROPERTY)='') AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");  //pong coa original
                //All null
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (LANDLORD_COMPANY is Null Or Trim(LANDLORD_COMPANY)='') AND (PROPERTY is Null Or Trim(PROPERTY)='') AND (CHARGE_CODE is Null Or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')" + " AND Upper(Trim(ptype))=Upper('" + ptype.Trim() + "') ");  //pong coa
                sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (LANDLORD_COMPANY is Null Or Trim(LANDLORD_COMPANY)='') AND (PROPERTY is Null Or Trim(PROPERTY)='') AND (CHARGE_CODE is Null Or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='') And (Trim(ptype) is null or Trim(ptype)='')");  //pong coa
                //sqlstr.Add("Select Seg_Value from COA_Product_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (LANDLORD_COMPANY is Null Or Trim(LANDLORD_COMPANY)='') AND (PROPERTY is Null Or Trim(PROPERTY)='') AND (CHARGE_CODE is Null Or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");  //pong coa original
            }
            //
            while (ckResult.Length < 1 && i < sqlstr.Count)
            {
                for (i = 0; i < sqlstr.Count; i++)
                {
                    if (ckResult == "")
                    {
                        dt = this._db.ExecuteDT(sqlstr[i].ToString());
                        ckResult = CheckDT(dt);
                    }
                }

            }
            if (ckResult.Length > 0)
            {
                _product_code = ckResult;
                result = true;
            }
            else
            {
                Exception e = new Exception("9903 - Get Product Code failed,please check COA Mapping setting. Parameter is : AccountId-" + accid + ", landlord-" + landlord + ", property-" + property + ", charge_code-" + charge_code + ", billing-" + billing + ", receipt_type-" + receipt_type + ", receipt_Company-" + receipt + ", bank-" + bank + ", action-" + action);
                result = false;
                throw e;
            }
        }
        catch (Exception ex)
        {
            result = false;
            throw ex;
        }
        return result;

    }

    public bool GetCountry()
    {
        bool result = false;
        try
        {
            String sqlstr = "Select Default_Value from AD_COA_Seg Where Upper(TRIM(SEGMENT_CODE))=Upper('cty')";
            DataTable dt = this._db.ExecuteDT(sqlstr);
            if (dt != null)
            {
                if (dt.Rows.Count > 1)
                {
                    Exception e = new Exception("9904 - Get default Country failed, please check COA Mapping setting.");
                    // e.Source = "class Segment, method GetCountry()";
                    // e.Message = "9904";
                    result = false;
                    throw e;
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    _country_code = dt.Rows[0]["Default_Value"].ToString();
                }
                result = true;
            }
        }
        catch (Exception ex)
        {
            result = false;
            throw ex;
        }
        return result;
    }

    public bool GetCountry(string accid, string landlord, string property, string billing, string receipt, string bank, string action)
    {
        bool result = false;
        string ckResult = "";
        DataTable dt = new DataTable();
        ArrayList sqlstr = new ArrayList();
        int i = 0;
        try
        {
            // 102, Landlord Company, Income, Landlord + Property
            if (accid == "102")
            {
                sqlstr.Add("Select Seg_Value from COA_Country_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "')   And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Country_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) AND Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "')   And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Country_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND (Trim(PROPERTY)='' or Trim(PROPERTY) is null)   And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Country_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) AND (Trim(PROPERTY)='' or Trim(PROPERTY) is null)   And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
            }
            // 103, Landlord Company, Bank(with lease), Receipt Company + Bank
            // 110, Landlord Company, Suspense(with lease), Receipt Company + Bank
            // 302, Receipt Company, Bank(With Lease), Receipt Company + Bank
            // 312, Receipt Company, Suspense(With Lease), Receipt Company + Bank
            if (accid == "103" || accid == "110" || accid == "302" || accid == "312")
            {
                sqlstr.Add("Select Seg_Value from COA_Country_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Country_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Country_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND (Trim(Bank)='' or Trim(Bank) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Country_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND (Trim(Bank)='' or Trim(Bank) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(ACT) is null or Trim(ACT)='')");
            }
            // 106, Landlord Company, Deposit, Landlord + Property
            // 107, Landlord Company, Deposit refund, Landlord + Property
            if (accid == "106" || accid == "107")
            {
                sqlstr.Add("Select Seg_Value from COA_Country_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "')   And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Country_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) AND Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "')   And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Country_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND (Trim(PROPERTY)='' or Trim(PROPERTY) is null)   And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Country_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) AND (Trim(PROPERTY)='' or Trim(PROPERTY) is null)   And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
            }
            // 108, Landlord Company, Bad debt/Other Income, "Action" field
            if (accid == "108")
            {
                sqlstr.Add("Select Seg_Value from COA_Country_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(Act))=Upper('" + action.Trim() + "')    And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(BANK) is null or Trim(BANK)='')");
                sqlstr.Add("Select Seg_Value from COA_Country_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(Act)='' or Trim(Act) is null)    And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(BANK) is null or Trim(BANK)='')");
            }
            // 202, Billing Company, Income, Billing Company
            // 206, Billing Company, Inter-co, Billing Company
            if (accid == "202" || accid == "206")
            {
                sqlstr.Add("Select Seg_Value from COA_Country_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "')   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from COA_Country_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null)   And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
            }
            //501,Rent-Free Income,Dr(Rent-Free Allowance),Landlord Company+Property
            //502,Rent-Free Income,Cr(Rental Income),Landlord Company+Property
            //503,Rent-Free Amortization,Dr(Rent-Free Amortization),Landlord Company+Property
            //504,Rent-Free Amortization,Cr(Accumulated Amortization),Landlord Company+Property
            //505,Rent Incentive Amortization,Dr(Rent Incentive Amortization),Landlord Company+Property
            //506,Rent Incentive Amortization,Cr(Rent Incentive Allowance),Landlord Company+Property
            //601,Step-Rent Income,Dr(Step Rent),Landlord Company+Property
            //602,Step-Rent Income,Cr(Rental Income),Landlord Company+Property
            //603,Step-Rent Amortization,Dr(Step Rent Amortization),Landlord Company+Property
            //604,Step-Rent Amortization,Cr(Acc Amort-Step Rent),Landlord Company+Property
            if (accid == "501" || accid == "502" || accid == "503" || accid == "504" || accid == "505" || accid == "506" || accid == "601" || accid == "602" || accid == "603" || accid == "604")
            {
                //Landlord Company+Property
                sqlstr.Add("Select Seg_Value from COA_Country_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "')   And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                //Only Landlord Company
                sqlstr.Add("Select Seg_Value from COA_Country_Code Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND (PROPERTY is null or Trim(PROPERTY)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                //Only Property
                sqlstr.Add("Select Seg_Value from COA_Country_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (LANDLORD_COMPANY is null or Trim(LANDLORD_COMPANY)='') AND Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "')   And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                //All null
                sqlstr.Add("Select Seg_Value from COA_Country_Code Where ACCOUNT_ID=To_Number('" + accid + "') and (LANDLORD_COMPANY is null or Trim(LANDLORD_COMPANY)='') AND (PROPERTY is null or Trim(PROPERTY)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
            }
            //
            while (ckResult.Length < 1 && i < sqlstr.Count)
            {
                for (i = 0; i < sqlstr.Count; i++)
                {
                    if (ckResult == "")
                    {
                        dt = this._db.ExecuteDT(sqlstr[i].ToString());
                        ckResult = CheckDT(dt);
                    }

                }
            }
            if (ckResult.Length > 0)
            {
                _country_code = ckResult;
                result = true;
            }
            else
            {
                Exception e = new Exception("9904-Get Country failed, please check COA Mapping setting. Parameter is AccountId-" + accid + ", landlord-" + landlord + ", property-" + property + ", billing-" + billing + ", receipt-" + receipt + ", bank-" + bank + ", action-" + action);
                result = false;
                throw e;
            }
        }
        catch (Exception ex)
        {
            result = false;
            throw ex;
        }
        return result;
    }

    public bool GetInter()
    {
        bool result = false;
        try
        {
            String sqlstr = "Select Default_Value from AD_COA_Seg Where UPPER(TRIM(SEGMENT_CODE))=UPPER('icp')";
            DataTable dt = this._db.ExecuteDT(sqlstr);
            if (dt != null)
            {
                if (dt.Rows.Count > 1)
                {
                    Exception e = new Exception("9905-Get default Inter-Co failed, please check COA Mapping setting.");
                    // e.Source = "class Segment, method GetInter()";
                    // e.Message = "9905";
                    result = false;
                    throw e;
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    _inter_code = dt.Rows[0]["Default_Value"].ToString();
                }
                result = true;
            }
        }
        catch (Exception ex)
        {
            result = false;
            throw ex;
        }
        return result;
    }

    public bool GetSales(string accid, string landlord, string property, string charge_code, string billing, string receipt_type, string receipt, string bank, string action)
    {
        bool result = false;
        string ckResult = "";
        DataTable dt = new DataTable();
        ArrayList sqlstr = new ArrayList();
        int i = 0;
        try
        {
            // 101, landlord company, Debtor, Landlord Company + Property + Charge Code 
            // 102, landlord company, Income, Landlord Company + Property + Charge Code
            // 106, landlord company, Deposit, Landlord Company + Property + Charge Code
            // 107, landlord company, Deposit refund, Landlord Company + Property + Charge Code
            if (accid == "101" || accid == "102" || accid == "106" || accid == "107")
            {
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "') AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Upper(Trim(LANDLORD_COMPANY))='' or Trim(LANDLORD_COMPANY) is null) AND Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "') AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND (Trim(PROPERTY)='' or Trim(PROPERTY) is null) AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) AND (Trim(PROPERTY)='' or Trim(PROPERTY) is null)   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "') And (Upper(Trim(CHARGE_CODE))='' or Trim(CHARGE_CODE) is null)   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord + "') AND (Trim(PROPERTY)='' or Trim(PROPERTY) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null)   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(PROPERTY)='' or Trim(PROPERTY) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null)   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
            }
            // 103, landlord company, Bank(with lease), Receipt Company + Bank
            // 110, landlord company, Suspense(with lease), Receipt Company + Bank
            // 302, Receipt Company, Bank(With Lease), Receipt Company + Bank
            // 303, Receipt Company, Bank (Without Lease), Receipt Company + bank
            // 312, Receipt Company, Suspense(With Lease), Receipt Company + Bank
            // 313, Receipt Company, Suspense (Without Lease), Receipt Company + bank
            // 304, Receipt Company, Bank (Unidentified), Receipt Company + Bank
            if (accid == "103" || accid == "110" || accid == "302" || accid == "303" || accid == "312" || accid == "313" || accid == "304")
            {
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "')  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "')  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "')  AND (Trim(Bank)='' or Trim(Bank) is null)  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null)  AND (Trim(Bank)='' or Trim(Bank) is null)  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
            }
            // 104, landlord company, C/A with Receipt Com(With Lease), Landlord Company + Receipt Company
            // 105, landlord company, C/A with Parent Com (With Lease), Landlord Company + Receipt Company
            // 305, Receipt Company, C/A with Parent Com (With Lease), Receipt Company + Landlord Company
            // 309, Receipt Company, C/A with landlord, Receipt Company + Landlord Company 
            if (accid == "104" || accid == "105" || accid == "305" || accid == "309")
            {
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "')  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='') And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null)  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='') And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "')  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='') And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null)  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='') And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
            }
            // 108, landlord company, Bad debt/Other Income, "Action" field
            if (accid == "108")
            {
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(Act))=Upper('" + action.Trim() + "')   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(Act)='' or Trim(Act) is null)   And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
            }

            // 109, landlord company, Unidentified Receipt, Receipt Company + Receipt Type
            // 205, Billing Company, Unidentified Receipt, Receipt Company + Receipt Type
            // 207, Billing Company, Suspense, Receipt Company + Receipt Type
            // 307, Receipt Company, Unidentified Receipt,Receipt Company + Receipt Type
            if (accid == "109" || accid == "205" || accid == "207" || accid == "307")
            {
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "')  And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "')  And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null)  And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null)  And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
            }
            // 201, Billing Company, Debtor, Billing Company + Charge Code
            // 202, Billing Company, Income, Billing Company + Charge Code
            // 301, Receipt Company, Debtor, Billing Company + Charge Code
            if (accid == "201" || accid == "202" || accid == "301")
            {
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "')  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null)  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='')");
            }

            // 203, Billing Company, C/A with Receipt Com(Without Lease), Billing Company + Receipt Company
            // 204, Billing Company, C/A with Parent Com (Without Lease), Billing Company + Receipt Company
            // 306, Receipt Company, C/A with Parent Com (Without Lease),Receipt Company + Billing Company
            // 310, Receipt Company, C/A with Billing Company, Receipt Company + Billing Company
            // 402, Parent Company, C/A with Receipt Com(Without Lease), Billing Company + Receipt Company
            // 404, Parent Company, C/A with Billing Company, Billing Company + Receipt Company
            if (accid == "203" || accid == "204" || accid == "306" || accid == "310" || accid == "402" || accid == "404")
            {
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')  And (Trim(PROPERTY) is null or Trim(PROPERTY)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
            }
            // 206, Billing Company, Inter-co, Billing Company + Bank + Receipt type + Charge Code
            if (accid == "206")
            {
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) And Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) And (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) And (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");

                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) And Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(Bank)='' or Trim(Bank) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) And Upper(Trim(Bank))=Upper('" + bank.Trim() + "') AND (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");

                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') And (Trim(Bank)='' or Trim(Bank) is null) AND (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) And Upper(Trim(Bank))=Upper('" + bank.Trim() + "') AND (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) And (Trim(Bank)='' or Trim(Bank) is null) AND Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) And (Trim(Bank)='' or Trim(Bank) is null) AND (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
            }
            // 308, Receipt Company, Inter-co, Receiving Company  + Bank + Receipt type + Charge Code
            if (accid == "308")
            {
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");

                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");

                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='') ");
            }
            // 311, Receipt Company, C/A with customer, Receipt Company + Billing Company + Bank + Receipt type + Charge Code
            if (accid == "311")
            {
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') and Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) and Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) and (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) and (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) and (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");

                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') and Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') and Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') and (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) and Upper(Trim(Bank))=Upper('" + bank.Trim() + "') and Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");

                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') And (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");

                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') and Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') And (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') And (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");

                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') AND (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND Upper(Trim(Bank))=Upper('" + bank.Trim() + "') And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND (Trim(Bank)='' or Trim(Bank) is null) And Upper(Trim(RECEIPT_TYPE))=Upper('" + receipt_type.Trim() + "') And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND Upper(Trim(BILLING_COMPANY))=Upper('" + billing.Trim() + "') AND (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) AND (Trim(BILLING_COMPANY)='' or Trim(BILLING_COMPANY) is null) AND (Trim(Bank)='' or Trim(Bank) is null) And (Trim(RECEIPT_TYPE)='' or Trim(RECEIPT_TYPE) is null) And (Trim(CHARGE_CODE)='' or Trim(CHARGE_CODE) is null) And (Trim(PROPERTY) is null or Trim(PROPERTY)='')  And (Trim(ACT) is null or Trim(ACT)='')");
            }
            // 401, Parent Company, C/A with Receipt Com(With Lease), Landlord Company + Receipt Co + Property
            // 403, Parent Company, C/A with landlord, Landlord Company + Receipt Co + Property
            if (accid == "401" || accid == "403")
            {
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "')  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) AND Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "') AND Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "')  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND (Trim(PROPERTY)='' or Trim(PROPERTY) is null) AND Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "')  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) AND (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "')  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");

                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And Upper(Trim(PROPERTY))=Upper('" + property.Trim() + "')  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And (Trim(PROPERTY)='' or Trim(PROPERTY) is null)  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) AND Upper(Trim(RECEIPT_COMPANY))=Upper('" + receipt.Trim() + "') And (Trim(PROPERTY)='' or Trim(PROPERTY) is null)  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (Trim(LANDLORD_COMPANY)='' or Trim(LANDLORD_COMPANY) is null) AND (Trim(RECEIPT_COMPANY)='' or Trim(RECEIPT_COMPANY) is null) And (Trim(PROPERTY)='' or Trim(PROPERTY) is null)  And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='') And (Trim(CHARGE_CODE) is null or Trim(CHARGE_CODE)='')");
            }
            //501,Rent-Free Income,Dr(Rent-Free Allowance),Landlord Company+Charge Code
            //502,Rent-Free Income,Cr(Rental Income),Landlord Company+Charge Code
            //503,Rent-Free Amortization,Dr(Rent-Free Amortization),Landlord Company+Charge Code
            //504,Rent-Free Amortization,Cr(Accumulated Amortization),Landlord Company+Charge Code
            //505,Rent Incentive Amortization,Dr(Rent Incentive Amortization),Landlord Company+Charge Code
            //506,Rent Incentive Amortization,Cr(Rent Incentive Allowance),Landlord Company+Charge Code
            //507,Step Rent Amortization,Dr(Step Rent Amortization),Landlord Company+Charge Code
            //508,Step Rent Amortization,Cr(Rental Income-step Rent),Landlord Company+Charge Code
            if (accid == "501" || accid == "502" || accid == "503" || accid == "504" || accid == "505" || accid == "506" || accid == "507" || accid == "508")
            {
                //Landlord Company + Charge Code
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND (PROPERTY is Null Or Trim(PROPERTY)='') AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                //Only Landlord Company
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and Upper(Trim(LANDLORD_COMPANY))=Upper('" + landlord.Trim() + "') AND (PROPERTY is Null Or Trim(PROPERTY)='') AND (CHARGE_CODE is Null Or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                //Only Charge Code
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (LANDLORD_COMPANY is Null Or Trim(LANDLORD_COMPANY)='') AND (PROPERTY is Null Or Trim(PROPERTY)='') AND Upper(Trim(CHARGE_CODE))=Upper('" + charge_code.Trim() + "') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
                //All null
                sqlstr.Add("Select Seg_Value from Coa_Sales_Channel Where ACCOUNT_ID=To_Number('" + accid + "') and (LANDLORD_COMPANY is Null Or Trim(LANDLORD_COMPANY)='') AND (PROPERTY is Null Or Trim(PROPERTY)='') AND (CHARGE_CODE is Null Or Trim(CHARGE_CODE)='') And (Trim(RECEIPT_TYPE) is null or Trim(RECEIPT_TYPE)='') And (Trim(BANK) is null or Trim(BANK)='') And (Trim(ACT) is null or Trim(ACT)='')");
            }
            //
            while (ckResult.Length < 1 && i < sqlstr.Count)
            {
                for (i = 0; i < sqlstr.Count; i++)
                {
                    if (ckResult == "")
                    {
                        dt = this._db.ExecuteDT(sqlstr[i].ToString());
                        ckResult = CheckDT(dt);
                    }
                }

            }
            if (ckResult.Length > 0)
            {
                _sales = ckResult;
                result = true;
            }
            else
            {
                Exception e = new Exception("9909- Get sales channel failed,please check COA Mapping setting. Parameter is : AccountId-" + accid + ", landlord-" + landlord + ", property-" + property + ", charge_code-" + charge_code + ", billing-" + billing + ", receipt_type-" + receipt_type + ", receipt_Company-" + receipt + ", bank-" + bank + ", action-" + action);
                result = false;
                throw e;
            }
        }
        catch (Exception ex)
        {
            result = false;
            throw ex;
        }
        return result;
    }

    protected string CheckDT(DataTable dt)
    {
        string result = "";
        try
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                if (dt.Rows.Count > 1)
                {
                    Exception e = new Exception("9990");
                    throw e;
                }
                else if (dt.Rows.Count == 1)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        result = dt.Rows[0][0].ToString();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        if (result == null)
            result = "";
        return result.Trim();
    }
}