﻿using Oqtane.Models;
using System.Threading.Tasks;
using System.Net.Http;
using System.Linq;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using Oqtane.Shared;
using System;

namespace Oqtane.Services
{
    public class SettingService : ServiceBase, ISettingService
    {
        private readonly HttpClient http;
        private readonly SiteState sitestate;
        private readonly IUriHelper urihelper;

        public SettingService(HttpClient http, SiteState sitestate, IUriHelper urihelper)
        {
            this.http = http;
            this.sitestate = sitestate;
            this.urihelper = urihelper;
        }

        private string apiurl
        {
            get { return CreateApiUrl(sitestate.Alias, urihelper.GetAbsoluteUri(), "Setting"); }
        }

        public async Task<List<Setting>> GetModuleSettingsAsync(int ModuleId)
        {
            return await GetSettingsAsync("Module", ModuleId);
        }

        public async Task<Setting> UpdateModuleSettingsAsync(List<Setting> ModuleSettings, int ModuleId, string SettingName, string SettingValue)
        {
            Setting setting = ModuleSettings.Where(item => item.SettingName == SettingName).FirstOrDefault();
            if (setting == null)
            {
                setting = new Setting();
                setting.EntityName = "Module";
                setting.EntityId = ModuleId;
                setting.SettingName = SettingName;
                setting.SettingValue = SettingValue;
                setting = await AddSettingAsync(setting);
            }
            else
            {
                setting.SettingValue = SettingValue;
                setting = await UpdateSettingAsync(setting);
            }
            return setting;
        }

        public async Task<List<Setting>> GetSettingsAsync(string EntityName, int EntityId)
        {
            List<Setting> Settings = await http.GetJsonAsync<List<Setting>>(apiurl + "?entityname=" + EntityName + "&entityid=" + EntityId.ToString());
            return Settings.OrderBy(item => item.SettingName).ToList();
        }

        public async Task<Setting> GetSettingAsync(int SettingId)
        {
            return await http.GetJsonAsync<Setting>(apiurl + "/" + SettingId.ToString());
        }

        public async Task<Setting> AddSettingAsync(Setting Setting)
        {
            return await http.PostJsonAsync<Setting>(apiurl, Setting);
        }

        public async Task<Setting> UpdateSettingAsync(Setting Setting)
        {
            return await http.PutJsonAsync<Setting>(apiurl + "/" + Setting.SettingId.ToString(), Setting);
        }
        public async Task DeleteSettingAsync(int SettingId)
        {
            await http.DeleteAsync(apiurl + "/" + SettingId.ToString());
        }


        public string GetSetting(List<Setting> Settings, string SettingName, string DefaultValue)
        {
            string value = DefaultValue;
            Setting setting = Settings.Where(item => item.SettingName == SettingName).FirstOrDefault();
            if (setting != null)
            {
                value = setting.SettingValue;
            }
            return value;
        }
    }
}