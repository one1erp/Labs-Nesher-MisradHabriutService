using System;
using FCS_OBJECTS;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Net;
using System.Threading;
using System.Text;
using Common;
using DAL;


namespace MisradHabriutService
{
    public partial class ListenerRequests
    {

        public ListenerRequests(List<string> args)
        {


            Console.WriteLine("Starting HTTP listener...");
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://*:8080/");
            if (args.Count > 0)
            {
                foreach (var s in args)
                {
                    listener.Prefixes.Add(s);
                }
            }
            listener.Start();
            while (true)
            {
                HttpListenerContext ctx = listener.GetContext();
                ThreadPool.QueueUserWorkItem((_) =>
                {
                    try
                    {

                        Logger.WriteLogFile($"{ctx.Request.Url}");
                        Console.WriteLine($"{ctx.Request.Url}");

                        string methodName = ctx.Request.Url.Segments[1].Replace("/", "");
                        string value = ctx.Request.Url.Segments[2];


                        string[] strParams = ctx.Request.Url
                                                .Segments
                                                .Skip(2)
                                                .Select(s => s.Replace("/", ""))
                                                .ToArray();

                        var method = this.GetType().GetMethod(methodName);
                        object[] @params = method.GetParameters()
                                            .Select((p, i) => Convert.ChangeType(strParams[i], p.ParameterType))
                                            .ToArray();

                        object ret = method.Invoke(this, @params);
                        if (ret is string)
                        {
                            var json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(ret);
                            byte[] b = Encoding.UTF8.GetBytes(json /*res.str*/);
                            ctx.Response.OutputStream.Write(b, 0, b.Length);
                            Console.WriteLine("StatusCode " + ctx.Response.StatusCode);
                            ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                            ctx.Response.StatusDescription = ret.ToString();
                            ctx.Response.AddHeader("messages", ret.ToString());
                            
                            ctx.Response.ContentType = "text/plain";
                            ctx.Response.OutputStream.Close();

                        }
                        //if (ret is ResponseServise)
                        //{
                        //    var json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(ret);
                        //    Console.WriteLine(json);
                        //    ResponseServise res = (ResponseServise)ret;
                        //    Write("success - " + res.Success);
                        //    //if (res.Success == false)
                        //    //{
                        //    //    ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                        //    //}
                        //    //else
                        //    //{
                        //    //    ctx.Response.StatusCode = (int)HttpStatusCode.NotAcceptable;

                        //    //    //byte[] b = Encoding.UTF8.GetBytes(json /*res.FCS_MSG_ID + "\n" + res.str + "\n" + res.ErrDesc);*/);
                        //    //    //ctx.Response.OutputStream.Write(b, 0, b.Length);
                        //    //}
                        //    byte[] b = Encoding.UTF8.GetBytes(json /*res.str*/);
                        //    ctx.Response.OutputStream.Write(b, 0, b.Length); 
                        //    Console.WriteLine("StatusCode " + ctx.Response.StatusCode);
                        //    Console.WriteLine("res.err -" + res.Error);

                        //    ctx.Response.StatusDescription = res.ErrDesc;
                        //    ctx.Response.AddHeader("FCS_MSG_ID", res.FCS_MSG_ID);
                        //    ctx.Response.AddHeader("messages", res.Error);

                        //}
                        //else
                        //{
                        //    ctx.Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        //}
                        //ctx.Response.ContentType = "text/plain";
                        //ctx.Response.OutputStream.Close();
                    }
                    catch (Exception e)
                    {

                        string MSG = "MisradHabriutService:  " + e.Message;
                        Write(MSG);
                    }
                });

            }
        }

        

        //   public ResponseServise FcsSampleRequest(string barcode)
        public string FcsSampleRequest(string barcode)
        {



            //ResponseServise res = null; //= new responseServise(false, null, null);
            //FCS_OBJECTS.ResponseServise res = new FCS_OBJECTS.ResponseServise(false, null, null, null);
            try
            {
                Write("Start  FcsSampleRequest");

                SampReqLogic FcsLogic = new SampReqLogic();
                var res = FcsLogic.FcsSampleRequest(barcode);
                //return res2;
                string ErrDesc = res.ErrDesc;
                if (ErrDesc is null)
                {
                    ErrDesc = "";
                }
                else
                {
                    ErrDesc = ErrDesc.Replace(';', ' ');
                }
                string Error = res.Error;
                if (Error is null)
                {
                    Error = "";
                }
                else
                {
                    Error = Error.Replace(';', ' ');
                }
                string returnstr = ErrDesc + " " + Error + ";" + res.Success.ToString() + ";" + res.FCS_MSG_ID + ";";
                return returnstr;
            }
            catch (Exception e)
            {

                //Write("FcsSampleRequest " + e.Message);
                //Write("MisradHabriutService:  " + e.Message);
                //res.str += "MisradHabriutService:  " + e.Message;
                //return res;

                return "MisradHabriutService:  " + e.Message + ";false;;";
            }

        }


        public FCS_OBJECTS.ResponseServise FcsResultRequest(string coaId)
        {
            Console.WriteLine("Start FcsResultRequest");
            ResponseServise res = null; ;//= new responseServise(false, null, null);
            try
            {
                ResultReqLogicCls fcsLogic = new ResultReqLogicCls();
                res = fcsLogic.FcsResultRequest(coaId);
                return res;
            }
            catch (Exception e)
            {
                Write("FcsResultRequest Error : " + e.Message);

                Write("MisradHabriutService: Error :" + e.Message);
                res.str += "MisradHabriutService: Error : " + e.Message;
                return res;

            }
        }






        void Write(string s)
        {
            Console.WriteLine(s);
            Logger.WriteLogFile(s);
        }
    }
}