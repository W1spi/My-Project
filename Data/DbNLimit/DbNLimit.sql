/*
** Copyright Microsoft, Inc. 1994 - 2000
** All Rights Reserved.
*/

DROP TABLE IF EXISTS "Teachers";
DROP TABLE IF EXISTS "Students";
DROP TABLE IF EXISTS "Courses";
DROP TABLE IF EXISTS "CoursesStudents";
DROP TABLE IF EXISTS "Tasks";

CREATE TABLE "Teachers" (
	"TeacherId" INTEGER PRIMARY KEY,
	"LastName" nvarchar (20) NOT NULL ,
	"FirstName" nvarchar (20) NOT NULL ,
	"MiddleName" nvarchar (20) NULL ,
	"BirthDate" "datetime" NULL ,
	"StartDate" "datetime" NULL ,
	"Address" nvarchar (60) NULL ,
	"City" nvarchar (15) NULL ,
	"Region" nvarchar (15) NULL ,
	"Country" nvarchar (15) NULL ,
	"HomePhone" nvarchar (24) NULL ,
	"MobilePhone" nvarchar (24) NULL ,
	"Email" nvarchar (24) NULL ,
	"Photo" "image" NULL ,
	"Notes" "ntext" NULL ,
	"ReportsTo" "int" NULL ,
	"PhotoPath" nvarchar (255) NULL 
);

CREATE TABLE "Students" (
	"StudentId" INTEGER PRIMARY KEY,
	"LastName" nvarchar (20) NOT NULL ,
	"FirstName" nvarchar (20) NOT NULL ,
	"MiddleName" nvarchar (20) NULL ,
	"BirthDate" "datetime" NULL ,
	"StartDate" "datetime" NULL ,
	"Address" nvarchar (60) NULL ,
	"City" nvarchar (15) NULL ,
	"Region" nvarchar (15) NULL ,
	"Country" nvarchar (15) NULL ,
	"HomePhone" nvarchar (24) NULL ,
	"MobilePhone" nvarchar (24) NULL ,
	"Email" nvarchar (24) NULL ,
	"Photo" "image" NULL ,
	"Notes" "ntext" NULL ,
	"ReportsTo" "int" NULL ,
	"PhotoPath" nvarchar (255) NULL ,
	"TeacherId" "int" NULL ,
	CONSTRAINT "FK_Student_CoursesStudents" FOREIGN KEY 
	(
		"StudentId"
	) REFERENCES "CoursesStudents" (
		"StudentId"
	),
	CONSTRAINT "FK_Student_Teachers" FOREIGN KEY 
	(
		"TeacherId"
	) REFERENCES "Teachers" (
		"TeacherId"
	)
);

CREATE TABLE "Courses" (
	"CourseId" INTEGER PRIMARY KEY,
	"CourseName" nvarchar (30) NOT NULL ,
	"Description" "ntext" NULL ,
	"Picture" "image" NULL ,
	"TeacherId" "int" NULL,
	CONSTRAINT "FK_Courses_CoursesStudents" FOREIGN KEY 
	(
		"CourseId"
	) REFERENCES "CoursesStudents" (
		"CourseId"
	),
	CONSTRAINT "FK_Courses_Teachers" FOREIGN KEY 
	(
		"TeacherId"
	) REFERENCES "Teachers" (
		"TeacherId"
	)
);

CREATE TABLE "CoursesStudents" (
	"CourseId" INTEGER NOT NULL,
	"StudentId" INTEGER NOT NULL
);

CREATE INDEX "CourseId" ON "CoursesStudents"("CourseId");
CREATE INDEX "StudentId" ON "CoursesStudents"("StudentId");

CREATE TABLE "Tasks" (
	"TaskId" INTEGER PRIMARY KEY,
	"TaskName" nvarchar (40) NOT NULL ,
	"StudentId" "int" NULL ,
	"TeacherId" "int" NULL ,
	"CourseId" "int" NULL,
	CONSTRAINT "FK_Tasks_Teachers" FOREIGN KEY 
	(
		"TeacherId"
	) REFERENCES "Teachers" (
		"TeacherId"
	),
	CONSTRAINT "FK_Tasks_Students" FOREIGN KEY 
	(
		"StudentId"
	) REFERENCES "Students" (
		"StudentId"
	),
	CONSTRAINT "FK_Tasks_Courses" FOREIGN KEY 
	(
		"CourseId"
	) REFERENCES "Courses" (
		"CourseId"
	)
);

INSERT INTO "Teachers"("TeacherId", "LastName", "FirstName", "MobilePhone")
VALUES(27,'Степанищев','Иван','89518567085');
INSERT INTO "Courses"("CourseId", "CourseName", "TeacherId")
VALUES(1,'Современная кросс-платформенная разработка (С#, .NET)',27);