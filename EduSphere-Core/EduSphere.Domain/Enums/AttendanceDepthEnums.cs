namespace EduSphere.Domain.Enums;

public enum AttendanceDeviceType { Biometric = 0, Rfid = 1, Hybrid = 2 }
public enum AttendanceImportStatus { Uploaded = 0, Processing = 1, Completed = 2, CompletedWithErrors = 3, Failed = 4 }
public enum AttendancePunchDirection { CheckIn = 0, CheckOut = 1, BusBoarding = 2, BusAlighting = 3 }
public enum AttendanceExplanationStatus { Submitted = 0, Approved = 1, Rejected = 2 }
public enum BusAttendanceStatus { Boarded = 0, Alighted = 1, Missed = 2 }

