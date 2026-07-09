-- ============================================================================
-- DATABASE SCHEMA: Student Enrollment System
-- Description: Sets up the structure for students, courses, and registrations.
-- ============================================================================

-- ----------------------------------------------------------------------------
-- 1. Table: Students
-- Description: Stores primary identity and academic details for every student.
-- ----------------------------------------------------------------------------
CREATE TABLE Students (
    -- Unique internal identifier for each student. Automatically increments.
    StudentID INT PRIMARY KEY AUTO_INCREMENT,
    
    -- Official institutional registry identifier. Must be unique per student.
    RegistrationNumber VARCHAR(50) NOT NULL UNIQUE,
    
    -- Student's legal first name.
    FirstName VARCHAR(100) NOT NULL,
    
    -- Student's legal last name/surname.
    LastName VARCHAR(100) NOT NULL,
    
    -- The degree program or department the student belongs to (e.g., 'Computer Science').
    Program VARCHAR(150),
    
    -- Current academic year of study (e.g., 1, 2, 3, 4).
    Year INT
);


-- ----------------------------------------------------------------------------
-- 2. Table: Courses (Assumed Helper Table)
-- Description: Required to satisfy the Foreign Key constraint in Registrations.
-- ----------------------------------------------------------------------------
CREATE TABLE Courses (
    -- Unique identifier for each course.
    CourseID INT PRIMARY KEY AUTO_INCREMENT,
    
    -- The official code (e.g., 'CS101') and name of the course.
    CourseCode VARCHAR(20) NOT NULL UNIQUE,
    CourseName VARCHAR(150) NOT NULL
);


-- ----------------------------------------------------------------------------
-- 3. Table: Registrations
-- Description: A junction table managing the Many-to-Many relationship 
--              between Students and Courses. Tracks which student takes what course.
-- ----------------------------------------------------------------------------
CREATE TABLE Registrations (
    -- Unique identifier for each individual registration record.
    RegistrationID INT PRIMARY KEY AUTO_INCREMENT,
    
    -- Links to Students.StudentID. Cannot be blank.
    StudentID INT NOT NULL,
    
    -- Links to Courses.CourseID. Cannot be blank.
    CourseID INT NOT NULL,
    
    -- The date the student successfully enrolled in the course (YYYY-MM-DD).
    RegistrationDate DATE NOT NULL,

    -- FOREIGN KEY: Ensures StudentID must physically exist in the Students table.
    -- ON DELETE CASCADE: If a student profile is deleted, their registrations are wiped automatically.
    CONSTRAINT fk_student FOREIGN KEY (StudentID) 
        REFERENCES Students(StudentID) 
        ON DELETE CASCADE,

    -- FOREIGN KEY: Ensures CourseID must physically exist in the Courses table.
    -- ON DELETE CASCADE: If a course is deleted, all student enrollments for it are wiped automatically.
    CONSTRAINT fk_course FOREIGN KEY (CourseID) 
        REFERENCES Courses(CourseID) 
        ON DELETE CASCADE
);