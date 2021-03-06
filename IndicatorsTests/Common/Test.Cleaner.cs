﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockIndicators.Tests
{
    [TestClass]
    public class CleanerTests : TestBase
    {

        [TestMethod()]
        public void PrepareHistoryTest()
        {
            IEnumerable<Quote> h = Cleaners.PrepareHistory(history);

            // assertions

            // should always be the same number of results as there is history
            Assert.AreEqual(502, h.Count());

            // should always have index
            Assert.IsFalse(h.Where(x => x.Index == null || x.Index <= 0).Any());

            // last index should be 502
            Quote r = history.Where(x => x.Date == DateTime.Parse("12/31/2018")).FirstOrDefault();
            Assert.AreEqual(502, r.Index);
        }


        [TestMethod()]
        public void CleanBasicDataTest()
        {
            // compose basic data
            IEnumerable<BasicData> o = Cleaners.ConvertHistoryToBasic(history, "O");
            IEnumerable<BasicData> h = Cleaners.ConvertHistoryToBasic(history, "H");
            IEnumerable<BasicData> l = Cleaners.ConvertHistoryToBasic(history, "L");
            IEnumerable<BasicData> c = Cleaners.ConvertHistoryToBasic(history, "C");
            IEnumerable<BasicData> v = Cleaners.ConvertHistoryToBasic(history, "V");

            // remove index
            foreach (BasicData x in c) { x.Index = null; }

            // re-clean index
            c = Cleaners.PrepareBasicData(c);

            // assertions

            // should always be the same number of results as there is history
            Assert.AreEqual(502, c.Count());

            // should always have index
            Assert.IsFalse(c.Where(x => x.Index == null || x.Index <= 0).Any());

            // samples
            BasicData ro = o.Where(x => x.Index == 502).FirstOrDefault();
            BasicData rc = c.Where(x => x.Date == DateTime.Parse("12/31/2018")).FirstOrDefault();
            BasicData rh = h.Where(x => x.Index == 502).FirstOrDefault();
            BasicData rl = l.Where(x => x.Index == 502).FirstOrDefault();
            BasicData rv = v.Where(x => x.Index == 502).FirstOrDefault();

            // last index should be 502
            Assert.AreEqual(502, rc.Index);

            // last values should be correct
            Assert.AreEqual(244.92m, ro.Value);
            Assert.AreEqual(245.54m, rh.Value);
            Assert.AreEqual(242.87m, rl.Value);
            Assert.AreEqual(245.28m, rc.Value);
            Assert.AreEqual(147031456, rv.Value);
        }



        /* BAD HISTORY EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "No historical quotes.")]
        public void NoHistory()
        {
            List<Quote> badHistory = new List<Quote>();
            Cleaners.PrepareHistory(badHistory);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Duplicate date found.")]
        public void DuplicateHistory()
        {
            List<Quote> badHistory = new List<Quote>
            {
            new Quote { Date = DateTime.Parse("2017-01-03"), Open=(decimal)214.86, High=(decimal)220.33, Low=(decimal)210.96, Close=(decimal)216.99, Volume = 5923254 },
            new Quote { Date = DateTime.Parse("2017-01-04"), Open=(decimal)214.75, High=228, Low=(decimal)214.31, Close=(decimal)226.99, Volume = 11213471 },
            new Quote { Date = DateTime.Parse("2017-01-05"), Open=(decimal)226.42, High=(decimal)227.48, Low=(decimal)221.95, Close=(decimal)226.75, Volume = 5911695 },
            new Quote { Date = DateTime.Parse("2017-01-06"), Open=(decimal)226.93, High=(decimal)230.31, Low=(decimal)225.45, Close=(decimal)229.01, Volume = 5527893 },
            new Quote { Date = DateTime.Parse("2017-01-06"), Open=(decimal)228.97, High=(decimal)231.92, Low=228, Close=(decimal)231.28, Volume = 3979484 }
            };

            Cleaners.PrepareHistory(badHistory);
        }


        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "No historical basic data.")]
        public void NoBasicData()
        {
            List<BasicData> bd = new List<BasicData>();
            Cleaners.PrepareBasicData(bd);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Duplicate date found.")]
        public void DuplicateBasicData()
        {
            List<BasicData> bd = new List<BasicData>
            {
            new BasicData { Date = DateTime.Parse("2017-01-03"), Value=(decimal)214.86},
            new BasicData { Date = DateTime.Parse("2017-01-04"), Value=(decimal)214.75},
            new BasicData { Date = DateTime.Parse("2017-01-05"), Value=(decimal)226.42},
            new BasicData { Date = DateTime.Parse("2017-01-06"), Value=(decimal)226.93},
            new BasicData { Date = DateTime.Parse("2017-01-06"), Value=(decimal)228.97}
            };

            Cleaners.PrepareBasicData(bd);
        }

    }
}