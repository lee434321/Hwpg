using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// 接口3.3更改密码
/// </summary>
public class Biz33 : BizFactory
{
    Req33 req = null;
	public Biz33()
	{
	}

    public Biz33(string postStr)
    {
        this.BIZ_NAME = "接口3.3";
        this.req = Common.Deserialize<Req33>(postStr) as Req33;
    }

    public override string InvokeRequest()
    {
        throw new NotImplementedException();
    }
}