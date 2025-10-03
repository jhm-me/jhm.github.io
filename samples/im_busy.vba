Option Explicit

Sub ImBusy()

'jhm - 2024-08-08

Dim mtg As Object
Dim reqAttendee, optAttendee As Outlook.Recipient

Set mtg = Application.CreateItem(olAppointmentItem)
mtg.MeetingStatus = olMeeting
mtg.Subject = "Super Important Meeting That I Can't Miss"
mtg.Location = "My Office With The Door Closed"
mtg.Start = CDate(Now - TimeSerial(0, 0, DatePart("s", Now))) 'truncate seconds off current time
mtg.Duration = 30 'minutes
mtg.Sensitivity = olPrivate
Set reqAttendee = mtg.Recipients.Add("Firstname Lastname <flast@url.com>") '''''''''''''''''''''''''''ADD YOUR NAME HERE :)
reqAttendee.Type = olRequired

'Set optAttendee = mtg.Recipients.Add("First Last <flast@url.com>")
'optAttendee.Type = olOptional

'uncomment + change optional recipient if you need to save someone else from peril.

mtg.Display
mtg.Send

End Sub
