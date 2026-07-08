# University Registration System

A simple **University Registration System** developed using **C# Windows Forms** and a **relational database**. This project demonstrates fundamental concepts of **database systems**, **system analysis and design**, and **desktop application development**.

The system allows administrators to manage students and courses, while students can register for and drop courses.

---

## Project Objectives

The main objective of this project is to demonstrate the principles of a database-driven information system by implementing a simple university registration process.

Specifically, the project aims to:

- Manage student records
- Manage course records
- Register students for courses
- Allow students to drop registered courses
- Demonstrate relationships between database tables
- Apply CRUD (Create, Read, Update, Delete) operations
- Demonstrate primary keys and foreign keys
- Implement a Windows Forms graphical user interface

---

# Features

## Student Management

- Add students
- Update student information
- Delete students
- View student list

## Course Management

- Add courses
- Update courses
- Delete courses
- View available courses

## Course Registration

- Register a student for a course
- View registered courses
- Drop a registered course

---

# System Modules

1. Student Management
2. Course Management
3. Course Registration

---

# Technologies Used

- C#
- Windows Forms
- SQL Server (or SQL Server Express)
- ADO.NET
- Visual Studio

---

# Database Design

The system consists of three main tables.

## Students

| Field | Type |
|--------|------|
| StudentID | INT (PK) |
| RegistrationNumber | VARCHAR |
| FirstName | VARCHAR |
| LastName | VARCHAR |
| Program | VARCHAR |
| Year | INT |

---

## Courses

| Field | Type |
|--------|------|
| CourseID | INT (PK) |
| CourseCode | VARCHAR |
| CourseName | VARCHAR |
| Credits | INT |

---

## Registrations

| Field | Type |
|--------|------|
| RegistrationID | INT (PK) |
| StudentID | INT (FK) |
| CourseID | INT (FK) |
| RegistrationDate | DATE |

---

# Entity Relationship

```
Students
---------
StudentID (PK)
RegistrationNumber
FirstName
LastName
Program
Year
      |
      | 1
      |
      |
Registrations
-------------
RegistrationID (PK)
StudentID (FK)
CourseID (FK)
RegistrationDate
      |
      | Many
      |
Courses
--------
CourseID (PK)
CourseCode
CourseName
Credits
```

A student can register for many courses.

A course can have many students.

The **Registrations** table resolves the many-to-many relationship.

---

# System Workflow

```
Administrator
      │
      ▼
Add Students
      │
      ▼
Add Courses
      │
      ▼
Student Registers for Courses
      │
      ▼
Registration Stored in Database
      │
      ▼
Student Views Registered Courses
      │
      ▼
Student Drops Course (Optional)
```

---

# User Interface

The application contains the following Windows Forms:

- Student Management Form
- Course Management Form
- Course Registration Form
- Registration Report Form

---

# Functional Requirements

## Administrator

- Add student
- Edit student
- Delete student
- Add course
- Edit course
- Delete course
- View registrations

## Student

- View available courses
- Register for courses
- View registered courses
- Drop courses

---

# Database Concepts Demonstrated

This project demonstrates several important database concepts including:

- Relational database design
- Entity Relationship (ER) modeling
- Primary Keys
- Foreign Keys
- One-to-Many relationships
- Many-to-Many relationships
- Normalization
- Referential Integrity
- CRUD operations
- SQL Joins

---

# Learning Outcomes

By completing this project, the following concepts are demonstrated:

- Database design
- Desktop application development using Windows Forms
- Database connectivity using ADO.NET
- SQL queries
- Data validation
- Event-driven programming
- Object-Oriented Programming (OOP)
- System analysis and design

---

# Future Improvements

The following features can be added in future versions:

- User authentication
- Semester management
- Course prerequisites
- Maximum credit hour validation
- Timetable management
- Grade management
- GPA calculation
- Fee payment management
- Transcript generation
- Report printing

---

# Project Structure

```
UniversityRegistrationSystem
│
├── Forms
│   ├── StudentForm
│   ├── CourseForm
│   ├── RegistrationForm
│   └── ReportForm
│
├── Models
│   ├── Student
│   ├── Course
│   └── Registration
│
├── Database
│   ├── SQL Scripts
│   └── Connection
│
├── Services
│
├── Utilities
│
└── README.md
```

---

# Conclusion

The University Registration System is a simple desktop application that demonstrates the core functionality of a university registration process. It provides a practical example of relational database implementation, Windows Forms development, and CRUD operations while maintaining a clear and manageable scope suitable for learning and academic projects.