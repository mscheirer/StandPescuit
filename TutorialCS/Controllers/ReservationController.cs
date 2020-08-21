using Data;
using DayPilot.Web.Mvc.Json;
using System;
using System.Data;
using System.Web.Mvc;

public class ReservationController : Controller
{
    public ActionResult Edit(string id)
    {
        DataRow dr = Db.GetReservation(id);

        if (dr == null)
        {
            throw new Exception("The task was not found");
        }

        return View(new
        {
            Id = id,
            Text = dr["ReservationName"],
            Start = Convert.ToDateTime(dr["ReservationStart"]).ToShortDateString(),
            End = Convert.ToDateTime(dr["ReservationEnd"]).ToShortDateString(),
            Status = new SelectList(new SelectListItem[]
            {
                new SelectListItem { Text = "Nou", Value = "0"},
                new SelectListItem { Text = "Confirmat", Value = "1"},
                new SelectListItem { Text = "Sosit", Value = "2"},
                new SelectListItem { Text = "Plecat", Value = "3"}
            }, "Value", "Text", dr["ReservationStatus"]),
            Paid = new SelectList(new SelectListItem[]
            {
                new SelectListItem { Text = "0%", Value = "0"},
                new SelectListItem { Text = "50%", Value = "50"},
                new SelectListItem { Text = "100%", Value = "100"},
            }, "Value", "Text", dr["ReservationPaid"]),
            Resource = new SelectList(Db.GetRoomSelectList(), "Value", "Text", dr["StandID"])
        });
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult Edit(FormCollection form)
    {
        string id = form["Id"];
        string name = form["Text"];
        DateTime start = Convert.ToDateTime(form["Start"]).Date.AddHours(12);
        DateTime end = Convert.ToDateTime(form["End"]).Date.AddHours(12);
        string resource = form["Resource"];
        int paid = Convert.ToInt32(form["Paid"]);
        int status = Convert.ToInt32(form["Status"]);

        DataRow dr = Db.GetReservation(id);

        if (dr == null)
        {
            throw new Exception("The task was not found");
        }

        Db.UpdateReservation(id, name, start, end, resource, status, paid);

        return JavaScript(SimpleJsonSerializer.Serialize("OK"));
    }

    public ActionResult Create()
    {
        return View(new
        {
            Start = Convert.ToDateTime(Request.QueryString["start"]).ToShortDateString(),
            End = Convert.ToDateTime(Request.QueryString["end"]).ToShortDateString(),
            Resource = new SelectList(Db.GetRoomSelectList(), "Value", "Text", Request.QueryString["resource"])
        });
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult Create(FormCollection form)
    {
        DateTime start = Convert.ToDateTime(form["Start"]).Date.AddHours(12);
        DateTime end = Convert.ToDateTime(form["End"]).Date.AddHours(12);
        string text = form["Text"];
        string resource = form["Resource"];

        Db.CreateReservation(start, end, resource, text);
        return JavaScript(SimpleJsonSerializer.Serialize("OK"));
    }


}
