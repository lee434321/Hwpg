using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Operater
/// </summary>
public class Operater
{
    private string _create_by;
    private string _creation_date;
    private string _last_updated_by;
    private string _last_updated_date;

    public String Create_By
    {
        get { return _create_by; }
        set { _create_by = value; }
    }
    public String Creation_Date
    {
        get { return _creation_date; }
        set { _creation_date = value; }
    }
    public String Last_Updated_By
    {
        get { return _last_updated_by; }
        set { _last_updated_by = value; }
    }
    public String Last_Updated_Date
    {
        get { return _last_updated_date; }
        set { _last_updated_date = value; }
    }
}

public class LineDistribution : Operater
{
    private string _journal_line_number;
    private string _line_no;
    private string _transaction_type;
    private string _transaction_no;
    private double _amount;
    private string _debit_credit;
    private string _currency;
    private double _exchange_rate;
    private string _post_status;
    private string _remark;
    private String _company;
    private String _account;
    private String _cost_center;
    private String _product_code;
    private String _country_code;
    private String _inter_code;
    private String _sales;
    private String _spare;
    private String _project;

    public String Journal_Line_Number
    {
        get { return _journal_line_number; }
        set { _journal_line_number = value; }
    }

    public String Line_No
    {
        get { return _line_no; }
        set { _line_no = value; }
    }
    public String Transaction_Type
    {
        get { return _transaction_type; }
        set { _transaction_type = value; }
    }
    public String Transaction_No
    {
        get { return _transaction_no; }
        set { _transaction_no = value; }
    }
    public Double Amount
    {
        get { return _amount; }
        set { _amount = value; }
    }
    public String Debit_Credit
    {
        get { return _debit_credit; }
        set { _debit_credit = value; }
    }
    public String Currency
    {
        get { return _currency; }
        set { _currency = value; }
    }
    public Double Exchange_Rate
    {
        get { return _exchange_rate; }
        set { _exchange_rate = value; }
    }
    public String Post_Status
    {
        get { return _post_status; }
        set { _post_status = value; }
    }
    public String Remark
    {
        get { return _remark; }
        set { _remark = value; }
    }
    public String Company
    {
        get { return _company; }
        set { _company = value; }
    }
    public String Account
    {
        get { return _account; }
        set { _account = value; }
    }
    public String Cost
    {
        get { return _cost_center; }
        set { _cost_center = value; }
    }
    public String Product
    {
        get { return _product_code; }
        set { _product_code = value; }
    }
    public String Country
    {
        get { return _country_code; }
        set { _country_code = value; }
    }
    public String Inter
    {
        get { return _inter_code; }
        set { _inter_code = value; }
    }
    public String Sales
    {
        get { return _sales; }
        set { _sales = value; }
    }
    public String Spare
    {
        get { return _spare; }
        set { _spare = value; }
    }
    public String Project
    {
        get { return _project; }
        set { _project = value; }
    }

    public bool Add()
    {
        return true;
    }
    public bool Update()
    {
        return true;
    }
    public bool Delete()
    {
        return true;
    }
}