using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Kanban_API;
using Kanban_API.Models;
using AutoMapper;

namespace Kanban_API.Controllers
{
    public class ListsController : ApiController
    {
        // replaces the using database statement we usually use
        private KanbanEntities db = new KanbanEntities();

        // GET: api/Lists
        public IEnumerable<ListModel> GetLists()
        {
            return Mapper.Map<IEnumerable<ListModel>>(db.Lists);
        }
 
        // GET: api/Lists/5/Cards
        [Route("api/lists/{listId}/cards")]
        public IEnumerable<CardModel> GetCardForList(int listId)
        {
            var cards = db.Cards.Where(c => c.ListId == listId);

            return Mapper.Map<IEnumerable<CardModel>>(cards);
        }

        // GET: api/Lists/5
        [ResponseType(typeof(ListModel))]
        public IHttpActionResult GetList(int id)
        {
            List list = db.Lists.Find(id);
            if (list == null)
            {
                return NotFound();
            }

            return Ok(Mapper.Map<ListModel>(list));
        }

        // PUT: api/Lists/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutList(int id, ListModel list)
        {
            if (!ModelState.IsValid) //ModelState is an object that reads the object coming into the method, 
                                     //checks for any of the attributes and runs required validation
                                     // !ModelState means it's empty
            {
                // service-side validation
                return BadRequest(ModelState);
            }

            if (id != list.ListId)
            {
                return BadRequest();
            }

            var dbList = db.Lists.Find(id);

            // should only use automapping when making a model from an object, but not backwards
            dbList.Update(list);


            db.Entry(dbList).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ListExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            //put method is a one-way communication so it doesn't have to return anything
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Lists
        [ResponseType(typeof(ListModel))]
        public IHttpActionResult PostList(ListModel list)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dbList = new List(list);

            db.Lists.Add(dbList);
            db.SaveChanges();

            list.CreatedDate = dbList.CreatedDate;
            list.ListId = dbList.ListId;

            // returns the object that is created 
            return CreatedAtRoute("DefaultApi", new { id = dbList.ListId }, list);
        }

        // DELETE: api/Lists/5
        [ResponseType(typeof(ListModel))]
        public IHttpActionResult DeleteList(int id)
        {
            List list = db.Lists.Find(id);
            if (list == null)
            {
                return NotFound();
            }

            db.Lists.Remove(list);
            db.SaveChanges();

            return Ok(Mapper.Map<ListModel>(list));
        }

        // similar to the dispose that happens after a using statement
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ListExists(int id)
        {
            return db.Lists.Count(e => e.ListId == id) > 0;
        }
    }
}