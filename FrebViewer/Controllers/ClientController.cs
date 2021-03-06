﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;


namespace FrebViewer.Controllers
{
    public class GetListController : ApiController
    {
        public FrebViewer.Models.Grid Get()
        {
            NameValueCollection queryString = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            string sidx = queryString["sidx"];
            string sord = queryString["sord"];
            int page = int.Parse(queryString["page"]);
            int rows = int.Parse(queryString["rows"]);
            
            bool _search;
            bool.TryParse(queryString["_search"], out _search);
            string searchField = queryString["searchField"];
            string searchString = queryString["searchString"];
            string searchOper = queryString["searchOper"];
            string SearchValue = queryString["SearchValue"];

            //&searchField=FileName &searchString=abcd &searchOper=eq&filters=
            var files = new Files();
            IEnumerable<FrebViewer.Models.Header> headers;            

            if (SearchValue != "")
            {
                headers = files.GetHeaders(SearchValue);
            }
            else{
                headers = files.GetHeaders(); 
            }

            if (searchString!= null)
                searchString = searchString.ToLower();

            int isearchString;

            if (int.TryParse(searchString, out isearchString))
                isearchString = Int32.Parse(searchString);

            switch (searchOper)
            {
                case "eq":
                    if (searchField == "FileName") //hard-coded values should be modified
                        headers = headers.Where(x => x.FileName.ToLower().Equals(searchString)).ToList();
                    else if (searchField == "URL")
                        headers = headers.Where(x => x.URL.ToLower().Equals(searchString)).ToList();
                    else if (searchField == "Verb")
                        headers = headers.Where(x => x.Verb.ToLower().Equals(searchString)).ToList();
                    else if (searchField == "AppPoolName")
                        headers = headers.Where(x => x.AppPoolName.ToLower().Equals(searchString)).ToList();
                    else if (searchField == "StatusCode")
                        headers = headers.Where(x => x.StatusCode == isearchString).ToList();
                    else if (searchField == "TimeTaken")
                        headers = headers.Where(x => x.TimeTaken == isearchString).ToList();
                    break;
                case "ne":
                    if (searchField == "FileName")
                        headers = headers.Where(x => !x.FileName.ToLower().Equals(searchString)).ToList();
                    else if (searchField == "URL")
                        headers = headers.Where(x => !x.URL.ToLower().Equals(searchString)).ToList();
                    else if (searchField == "Verb")
                        headers = headers.Where(x => !x.Verb.ToLower().Equals(searchString)).ToList();
                    else if (searchField == "AppPoolName")
                        headers = headers.Where(x => !x.AppPoolName.ToLower().Equals(searchString)).ToList();
                    else if (searchField == "StatusCode")
                        headers = headers.Where(x => (x.StatusCode != isearchString)).ToList();
                    else if (searchField == "TimeTaken")
                        headers = headers.Where(x => (x.TimeTaken != isearchString)).ToList();
                    break;
                case "cn":
                    if (searchField == "FileName")
                        headers = headers.Where(x => x.FileName.ToLower().Contains(searchString)).ToList();
                    else if (searchField == "URL")
                        headers = headers.Where(x => x.URL.ToLower().Contains(searchString)).ToList();
                    else if (searchField == "Verb")
                        headers = headers.Where(x => x.Verb.ToLower().Contains(searchString)).ToList();
                    else if (searchField == "AppPoolName")
                        headers = headers.Where(x => x.AppPoolName.ToLower().Contains(searchString)).ToList();
                    break;
                case "ge":
                    if (searchField == "StatusCode")
                        headers = headers.Where(x => x.StatusCode >= isearchString).ToList();
                    else if (searchField == "TimeTaken")
                        headers = headers.Where(x => x.TimeTaken >= isearchString).ToList();
                    break;
                case "le":
                    if (searchField == "StatusCode")
                        headers = headers.Where(x => x.StatusCode <= isearchString).ToList();
                    else if (searchField == "TimeTaken")
                        headers = headers.Where(x => x.TimeTaken <= isearchString).ToList();
                    break;
            }

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;

            int totalRecords = headers.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            if (sord.ToUpper() == "DESC")
            {
                if (sidx == "FileName")
                    headers = headers.OrderByDescending(s => s.FileName);
                if (sidx == "URL")
                    headers = headers.OrderByDescending(s => s.URL);
                if (sidx == "Verb")
                    headers = headers.OrderByDescending(s => s.Verb);
                if (sidx == "AppPoolName")
                    headers = headers.OrderByDescending(s => s.AppPoolName);
                if (sidx == "StatusCode")
                    headers = headers.OrderByDescending(s => s.StatusCode);
                if (sidx == "TimeTaken")
                    headers = headers.OrderByDescending(s => s.TimeTaken);

                headers = headers.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                if (sidx == "FileName")
                    headers = headers.OrderBy(s => s.FileName);
                if (sidx == "URL")
                    headers = headers.OrderBy(s => s.URL);
                if (sidx == "Verb")
                    headers = headers.OrderBy(s => s.Verb);
                if (sidx == "AppPoolName")
                    headers = headers.OrderBy(s => s.AppPoolName);
                if (sidx == "StatusCode")
                    headers = headers.OrderBy(s => s.StatusCode);
                if (sidx == "TimeTaken")
                    headers = headers.OrderBy(s => s.TimeTaken);

                headers = headers.Skip(pageIndex * pageSize).Take(pageSize);
            }

            var jsonData = new FrebViewer.Models.Grid
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = headers
            };

            return jsonData;

        }       
  
    }
}