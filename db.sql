CREATE TABLE user_role (
    role_name VARCHAR(20) PRIMARY KEY
);

INSERT INTO user_role (role_name)
VALUES ('reader'), ('librarian');

CREATE TABLE Users (
    id_user INT IDENTITY(1,1) PRIMARY KEY,
    username VARCHAR(50) UNIQUE NOT NULL,
    password VARCHAR(355) NOT NULL,
    first_name VARCHAR(50) NOT NULL,
    last_name VARCHAR(50) NOT NULL,
    role VARCHAR(20) NOT NULL
        CONSTRAINT fk_user_role REFERENCES user_role(role_name)
);

CREATE TABLE Author (
    id_author INT IDENTITY(1,1) PRIMARY KEY,
    first_name VARCHAR(50) NOT NULL,
    last_name VARCHAR(50) NOT NULL
);

CREATE TABLE Category (
    id_category INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(100) UNIQUE NOT NULL
);

CREATE TABLE Book (
    id_book INT IDENTITY(1,1) PRIMARY KEY,
    title VARCHAR(100) NOT NULL,
    id_author INT NOT NULL
        CONSTRAINT fk_book_author REFERENCES Author(id_author)
            ON DELETE NO ACTION,
    id_category INT
        CONSTRAINT fk_book_category REFERENCES Category(id_category)
            ON DELETE SET NULL,
    isbn VARCHAR(13) UNIQUE NOT NULL,
	year INT NOT NULL,
    total_copies INT NOT NULL CHECK (total_copies >= 0),
    available_copies INT NOT NULL CHECK (available_copies >= 0),
    CHECK (available_copies <= total_copies),
    CHECK (LEN(isbn) = 13)
);


CREATE TABLE Loan (
    id_loan INT IDENTITY(1,1) PRIMARY KEY,
    id_book INT NOT NULL
        CONSTRAINT fk_loan_book REFERENCES Book(id_book)
            ON DELETE CASCADE,
    id_user INT NOT NULL
        CONSTRAINT fk_loan_user REFERENCES Users(id_user)
            ON DELETE CASCADE,
    loan_date DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    due_date DATE NOT NULL,
    return_date DATE,
    fine DECIMAL(6,2) DEFAULT 0.00 CHECK (fine >= 0),
    CHECK (due_date >= loan_date),
    CHECK (return_date IS NULL OR return_date >= loan_date)
);


INSERT INTO Author (first_name, last_name) VALUES
('Henryk', 'Sienkiewicz'),
('Adam', 'Mickiewicz'),
('Stanisław', 'Lem'),
('Olga', 'Tokarczuk'),
('Andrzej', 'Sapkowski');

INSERT INTO Category (name) VALUES
('Powieść historyczna'),
('Poemat'),
('Science Fiction'),
('Literatura współczesna'),
('Fantasy');


INSERT INTO Book (title, id_author, id_category, isbn, year, total_copies, available_copies) VALUES
('Quo Vadis', 1, 1, '9788373271890', 1896, 10, 10),
('Pan Tadeusz', 2, 2, '9788307032912', 1834, 8, 8),
('Solaris', 3, 3, '9788373191723', 1961, 12, 12),
('Bieguni', 4, 4, '9788308063007', 2007, 6, 6),
('Wiedźmin: Ostatnie życzenie', 5, 5, '9788375780635', 1993, 15, 15);


INSERT INTO Users (username, password, first_name, last_name, role) VALUES
('admin', 'admin1234', 'Jan', 'Kowalski', 'librarian'),
('anna', 'anna1234', 'Anna', 'Nowak', 'reader');
