using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymMembershipManagement.DataAccess.Services;
using Microsoft.Extensions.Hosting;

namespace GymMembershipManagement.DataAccess
{
    public class MembershipNotificationService : BackgroundService
    {
        private readonly MemberService _memberService;

        public MembershipNotificationService(MemberService memberService)
        {
            _memberService = memberService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _memberService.NotifyExpiringMembershipsAsync();
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken); // Provjera jednom dnevno
            }
        }
    }
}
