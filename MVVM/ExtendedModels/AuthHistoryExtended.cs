﻿using Cashbox.MVVM.ViewModels.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Cashbox.MVVM.Models
{
    public partial class AuthHistory
    {
        public AuthHistory() { }

        public static async Task<AuthHistoryViewModel?> NewAuthUser()
        {
            DateTime dateTime = new DateTime();
            HttpClient client = new HttpClient();
            try
            {
                HttpResponseMessage? response = await client.GetAsync("https://timeapi.io/api/TimeZone/zone?timeZone=Europe/Saratov");
                // не отвечает сервис
                if (!response.IsSuccessStatusCode)
                {
                    dateTime = DateTime.Now;
                    return CreateNewAuthDataTime(dateTime) ? new AuthHistoryViewModel(CashBoxDataContext.Context.AuthHistories.FirstOrDefault(x => x.Datetime == dateTime)!) : null;
                }
                // доступен сервис
                var resultJSON = JsonNode.Parse(await response.Content.ReadAsStringAsync());
                string? dataSTR = resultJSON?["currentLocalTime"]?.ToString();
                if (dataSTR != null)
                {
                    dateTime = DateTime.Parse(dataSTR);
                    return CreateNewAuthDataTime(dateTime) ? new AuthHistoryViewModel(CashBoxDataContext.Context.AuthHistories.FirstOrDefault(x => x.Datetime == dateTime)!) : null;
                }
                // dataSTR тупой return
                return null;
            }
            catch (HttpRequestException)
            {
                // нет интернета
                dateTime = DateTime.Now;
                return CreateNewAuthDataTime(dateTime) ? new AuthHistoryViewModel(CashBoxDataContext.Context.AuthHistories.FirstOrDefault(x => x.Datetime == dateTime)!) : null;
            }
        }
        public static bool CreateNewAuthDataTime(DateTime dataTime)
        {
            UserViewModel user = UserViewModel.GetCurrentUser()!;
            try
            {
                CashBoxDataContext.Context.Add(new AuthHistory()
                {
                    UserId = user.Id,
                    Datetime = dataTime,
                });
                var authCurrentUserHistory = CashBoxDataContext.Context.AuthHistories.Where(x => x.UserId == user.Id);
                if (authCurrentUserHistory.Count() >= 9)
                    CashBoxDataContext.Context.AuthHistories.Remove(authCurrentUserHistory.FirstOrDefault()!);
                CashBoxDataContext.Context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async static Task<List<AuthHistoryViewModel>> GetHistories()
        {
            return await CashBoxDataContext.Context.AuthHistories.Select(s => new AuthHistoryViewModel(s)).ToListAsync();
        }
    }
}
