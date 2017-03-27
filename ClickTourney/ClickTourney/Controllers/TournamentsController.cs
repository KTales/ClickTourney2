using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClickTourney.Models;
using Microsoft.AspNet.Identity;

namespace ClickTourney.Controllers
{
    public class TournamentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tournaments
        [Authorize]
        public ActionResult Index()
        {
            string currentUserId = User.Identity.GetUserId();
            return View(db.Tournaments.Where(t => t.Owner.Id == currentUserId).ToList());
        }

        // GET: Tournaments/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (!userIsParticipant(id))
            {
                ViewBag.tId = id;
                return View("~/Views/Error/404.cshtml");
            }

            Tournament tournament = db.Tournaments.Find(id);

            foreach (Match m in tournament.Matches.Where(m => m.Completed))
            {
                m.updateElimTourney();
                db.SaveChanges();
            }

            if (tournament == null)
            {
                return HttpNotFound();
            }
            return View(tournament);
        }

        private bool userIsParticipant(int? id)
        {
            Tournament tournament = db.Tournaments.Find(id);
            if (User.Identity.GetUserId() == tournament.Owner.Id)
                return true;

            foreach (Match m in tournament.Matches)
            {
                if (m.Player1.User != null)
                    if (m.Player1.User.Id == User.Identity.GetUserId())
                        return true;

                if (m.Player2.User != null)
                    if (m.Player2.User.Id == User.Identity.GetUserId())
                        return true;
            }
            return false;
        }

        // GET: Tournaments/Create
        [Authorize]
        public ActionResult Create()
        {
            IEnumerable<string> ValidTypes = new List<string> { "Round Robin", "Double Round Robin", "Elimination" };
            ViewData["ValidTypes"] = ValidTypes;
            return View();
        }

        // POST: Tournaments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "TournamentId,Name,PlayerCount,TournamentType, IsPublic")] Tournament tournament)
        {
            string ownerId = User.Identity.GetUserId();
            tournament.Owner = db.Users.FirstOrDefault(x => x.Id == ownerId);

            if (ModelState.IsValid)
            {
                // Get the player names entered
                List<string> playerNames = new List<string>();
                for (int i = 0; i < tournament.PlayerCount; ++i)
                {
                    if (Request != null && Request.Form != null)
                    {
                        if (Request.Form.Count < 3)
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                        if (Request.Form[i + 3] != "")
                            playerNames.Add(Request.Form[i + 3]);
                    }
                }

                db.Tournaments.Add(tournament);
                tournament.createMatches(playerNames);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(tournament);
        }

        // GET: Tournaments/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tournament tournament = db.Tournaments.Find(id);
            if (tournament == null)
            {
                return HttpNotFound();
            }
            return View(tournament);
        }

        // GET: Tournaments/Join/5
        public ActionResult Join(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Tournament tournament = db.Tournaments.Find(id);

            if (tournament == null)
            {
                return HttpNotFound();
            }

            if (tournament.IsPublic || (!tournament.IsPublic && User.Identity.IsAuthenticated))
            {
                return View(tournament);
            }
            else
            {
                ViewBag.ReturnUrl = Url.Action("Join", "Tournaments", new { id = id });
                return View("~/Views/Account/Login.cshtml");
            }
        }

        [HttpPost]
        // POST: Tournaments/Join/tourneyId
        public ActionResult Join(string button, int id)
        {
            Tournament tournament = db.Tournaments.Find(id);
            if (tournament == null)
            {
                return HttpNotFound();
            }

            try
            {
                int pId = Convert.ToInt32(button);
                Participant p = db.Participants.Where(x => x.ParticipantId == pId).Single();

                string userId = User.Identity.GetUserId();
                p.User = db.Users.Where(x => x.Id == userId).Single();
                p.Alias = p.User.DisplayName;

                db.Entry(p).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return RedirectToAction("Details", new { id=tournament.TournamentId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "TournamentId,Name,PlayerCount,TournamentType, IsPublic")] Tournament tournament)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tournament).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tournament);
        }

        [HttpPost]
        [Authorize]
        // POST: Participants/edit/
        public ActionResult updateParticipants()
        {
            try
            {
                for (int i = 0; i + 1 < Request.Form.Count; ++i)
                {
                    //Get the Id of the current Participant from the form
                    //Must do it this way or the LINQ expression bitches
                    int id = Convert.ToInt32(Request.Form[i]);

                    //Select a single record by ParticipantId
                    Participant party = db.Participants.Where(x => x.ParticipantId == id).Single();

                    //If the alias was changed update the record and mark it as changed
                    if (party.Alias != Request.Form[++i])
                    {
                        party.Alias = Request.Form[i];
                        db.Entry(party).State = EntityState.Modified;
                    }
                }
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return RedirectToAction("Index");
        }

        // GET: Tournaments/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tournament tournament = db.Tournaments.Find(id);
            if (tournament == null)
            {
                return HttpNotFound();
            }
            return View(tournament);
        }

        // POST: Tournaments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            Tournament tournament = db.Tournaments.Find(id);
            for (int i = 0; i < tournament.Matches.Count; ++i)
            {
                db.Matches.Remove(tournament.Matches.ElementAt(i));
            }
            db.Tournaments.Remove(tournament);
            db.SaveChanges();
            return RedirectToAction("Index");
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
