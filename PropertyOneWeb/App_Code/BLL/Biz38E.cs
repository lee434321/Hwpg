using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// 接口3.8E-获取Feedback类型
/// </summary>
public class Biz38E :BizFactory
{
    public Biz38E()
    { }

    public override string InvokeRequest()
    {
        throw new NotImplementedException();
    }

    private List<M_T_Feedback_Type> Select()
    {
        DbHelper db = new DbHelper(Common.OracleConnStrLocal, false);
        try
        {
            string sql = "select * from " + M_T_Feedback_Type.TableName;
            List<M_T_Feedback_Type> etyList = db.ExecuteList<M_T_Feedback_Type>(sql, null);
            return etyList;
        }
        catch (Exception err)
        { throw err; }
    }
}