using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC.Models;

namespace MVC.Utilities
{
    public class ServiceClass
    {
        public void A()
        {
            List<Person> a = new List<Person>();
            List<Person> b = new List<Person>();
            var sameornot = a.SequenceEqual(b);
            var diff = a.Except(b);
        }

        /***
         * LINQ LEFT JOIN
         * */
        //var orderForBooks = from bk in bookList
        //join ordr in bookOrders
        //on bk.BookID equals ordr.BookID
        //into a
        //from b in a.DefaultIfEmpty(new Order())
        //select new
        //{
        //    bk.BookID,
        //    Name = bk.BookNm,
        //    b.PaymentMode
        //};
    }
}