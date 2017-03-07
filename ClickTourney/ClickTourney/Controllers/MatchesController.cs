using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClickTourney.Models;

namespace ClickTourney.Controllers
{
    public class MatchesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Matches/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Match match = db.Matches.Find(id);
            if (match == null)
            {
                return HttpNotFound();
            }
            return View(match);
        }

        public ActionResult SaveWinner(int id, string command)
        {
            Match match = db.Matches.Find(id);

            if (command.Equals("player1"))
            {
                match.Completed = true;
                match.Winner = match.Player1;
                db.SaveChanges();
            }
            else
            {
                match.Completed = true;
                match.Winner = match.Player2;
                db.SaveChanges();
            }

            if(match.Tournament.TournamentType == "Elimination")
            {
                var nextMatch = match.Tournament.Matches.FirstOrDefault(m => m.PreviousMatch1 == match.MatchNumber || m.PreviousMatch2 == match.MatchNumber);
                if (nextMatch != null)
                {
                    if (nextMatch.Player1 == null || nextMatch.Player1 == match.Player1 || nextMatch.Player1 == match.Player2)
                    {
                        nextMatch.Player1 = match.Winner;
                    }
                    else
                    {
                        nextMatch.Player2 = match.Winner;
                    }
                }
            }
            db.SaveChanges();
            return RedirectToAction("Details/" + match.MatchId, "Matches");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
