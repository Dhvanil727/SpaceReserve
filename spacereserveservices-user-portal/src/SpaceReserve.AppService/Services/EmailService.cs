using System.Net;
using System.Net.Mail;
using AutoMapper;
using Microsoft.Extensions.Options;
using SpaceReserve.AppService.DTOs;
using SpaceReserve.Infrastructure.Contracts;
using SpaceReserve.Infrastructure.Entities;
using SpaceReserve.Utility.Settings;

namespace SpaceReserve.AppService.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSetting _emailSettings;
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;
        public EmailService(IOptions<EmailSetting> emailSettings, INotificationRepository notificationRepository, IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _emailSettings = emailSettings.Value;
            _mapper = mapper;
        }

        public async Task SendEmailAsync(EmailDto emailDto)
        {
            var smtpServer = _emailSettings.SmtpServer;
            var port = _emailSettings.Port;
            var senderEmail = _emailSettings.SenderEmail;
            var senderPassword = _emailSettings.SenderPassword;
            var enableSsl = _emailSettings.EnableSsl;

            using var smtpClient = new SmtpClient(smtpServer)
            {
                Port = port,
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = enableSsl
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail),
                Subject = emailDto.Subject,
                Body = emailDto.Body,
                IsBodyHtml = true
            };

            var ToEmailList = new List<string>();
            foreach (var userId in emailDto.ToUserId)
            {
                var email = await _notificationRepository.GetEmailByUserId(userId);
                if (email != null)
                {
                    ToEmailList.Add(email);
                }

            }
            if (ToEmailList != null)
            {
                foreach (var recipient in ToEmailList)
                {
                    if (!string.IsNullOrWhiteSpace(recipient))
                        mailMessage.To.Add(recipient);
                }
            }

            if (emailDto.CcList != null)
            {
                foreach (var recipient in emailDto.CcList)
                {
                    if (!string.IsNullOrWhiteSpace(recipient))
                        mailMessage.CC.Add(recipient);
                }
            }

            if (emailDto.BccList != null)
            {
                foreach (var recipient in emailDto.BccList)
                {
                    if (!string.IsNullOrWhiteSpace(recipient))
                        mailMessage.Bcc.Add(recipient);
                }
            }

            if (mailMessage.To.Count == 0 && mailMessage.CC.Count == 0 && mailMessage.Bcc.Count == 0)
                throw new ArgumentException("At least one recipient (To, CC, or BCC) must be specified");

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (SmtpException e)
            {
                throw new Exception("Failed to send email.", e);
            }

            try
            {
                if (emailDto.SenderSubjectId == null)
                {
                    emailDto.SenderId = _emailSettings.SystemUserId;
                }
                if(emailDto.SenderSubjectId != null)
                {
                    var senderId = await _notificationRepository.GetUserIdBySubjectId(emailDto.SenderSubjectId);
                    emailDto.SenderId = senderId;
                }
                if (emailDto.ToUserId != null)
                {
                    foreach (var recipient in emailDto.ToUserId)
                    {
                        var notificationDto = new NotificationDto
                        {
                            Subject = emailDto.Subject,
                            Body = emailDto.Body,
                            SenderId = emailDto.SenderId,
                            ReceiverId = recipient,
                            CreatedBy = emailDto.SenderId
                        };
                        var notificationModelData = _mapper.Map<NotificationModel>(notificationDto);
                        await _notificationRepository.AddNotifications(notificationModelData);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Failed to save notification. Database Error", e);
            }
        }
    }
}