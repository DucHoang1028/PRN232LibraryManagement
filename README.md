# Library Management System API

A comprehensive REST API for managing a library system with books, members, loans, reservations, and fines.

## Features

### Book Management
- Search books by title, author, category, or publication date
- Add, edit, and delete books
- Track book availability and copies
- Manage book categories and authors
- Support for barcodes and rack numbers

### Member Management
- Register and manage library members
- Generate library card numbers and barcodes
- Track member status and eligibility
- View member loans, reservations, and fines

### Loan Management
- Check out books to members
- Return books
- Renew loans
- Track overdue books
- Enforce loan limits (max 5 books per member)
- Set loan periods (default 10 days)

### Reservation System
- Reserve books when not available
- Track reservation status
- Process expired reservations
- Send notifications for available books

### Fine Management
- Calculate fines for overdue books
- Track fine payments
- Process overdue fines automatically

## API Endpoints

### Books

#### Get All Books
```
GET /api/Books
```

#### Get Book by ID
```
GET /api/Books/{id}
```

#### Search Books
```
GET /api/Books/search?title={title}&author={author}&category={category}&publicationDate={date}
```

#### Get Books by Author
```
GET /api/Books/author/{authorId}
```

#### Get Books by Category
```
GET /api/Books/category/{categoryId}
```

#### Get Books by Publisher
```
GET /api/Books/publisher/{publisherId}
```

#### Create Book
```
POST /api/Books
Content-Type: application/json

{
  "title": "Book Title",
  "isbn": "1234567890",
  "publisherId": "guid",
  "description": "Book description",
  "publicationDate": "2023-01-01",
  "language": "English",
  "totalCopies": 5,
  "availableCopies": 5,
  "rackNumber": "A1",
  "barcode": "BC123456"
}
```

#### Update Book
```
PUT /api/Books/{id}
Content-Type: application/json

{
  "title": "Updated Book Title",
  "isbn": "1234567890",
  "publisherId": "guid",
  "description": "Updated description",
  "publicationDate": "2023-01-01",
  "language": "English",
  "totalCopies": 5,
  "availableCopies": 3,
  "rackNumber": "A1",
  "barcode": "BC123456"
}
```

#### Delete Book
```
DELETE /api/Books/{id}
```

### Members

#### Get All Members
```
GET /api/Members
```

#### Get Member by ID
```
GET /api/Members/{id}
```

#### Get Member by Email
```
GET /api/Members/email/{email}
```

#### Get Member by Library Card
```
GET /api/Members/card/{libraryCardNumber}
```

#### Get Member Loans
```
GET /api/Members/{id}/loans
```

#### Get Member Reservations
```
GET /api/Members/{id}/reservations
```

#### Get Member Fines
```
GET /api/Members/{id}/fines
```

#### Check Member Checkout Eligibility
```
GET /api/Members/{id}/can-checkout
```

#### Get Member Active Loan Count
```
GET /api/Members/{id}/active-loan-count
```

#### Create Member
```
POST /api/Members
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "phone": "1234567890",
  "address": "123 Main St",
  "role": "Member"
}
```

#### Update Member
```
PUT /api/Members/{id}
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "phone": "1234567890",
  "address": "123 Main St",
  "role": "Member"
}
```

#### Delete Member
```
DELETE /api/Members/{id}
```

#### Deactivate Member
```
PUT /api/Members/{id}/deactivate
```

### Loans

#### Get All Loans
```
GET /api/Loans
```

#### Get Loan by ID
```
GET /api/Loans/{id}
```

#### Get Active Loans
```
GET /api/Loans/active
```

#### Get Overdue Loans
```
GET /api/Loans/overdue
```

#### Get Loans by Member
```
GET /api/Loans/member/{memberId}
```

#### Get Loans by Book
```
GET /api/Loans/book/{bookId}
```

#### Get Loans Due Today
```
GET /api/Loans/due-today
```

#### Get Loans Due in X Days
```
GET /api/Loans/due-in/{days}
```

#### Checkout Book
```
POST /api/Loans/checkout
Content-Type: application/json

{
  "bookId": "guid",
  "memberId": "guid"
}
```

#### Return Book
```
PUT /api/Loans/{id}/return
```

#### Renew Book
```
PUT /api/Loans/{id}/renew
```

#### Check Book Availability
```
GET /api/Loans/book/{bookId}/available
```

#### Check Member Eligibility
```
GET /api/Loans/member/{memberId}/eligible
```

#### Process Overdue Loans
```
POST /api/Loans/process-overdue
```

## Business Rules

### Book Management
- Books must have a title and ISBN
- Total copies cannot be negative
- Available copies cannot exceed total copies
- Books with active loans cannot be deleted

### Member Management
- Members must have first name, last name, and email
- Email addresses must be unique
- Members with active loans cannot be deleted
- Library card numbers and barcodes are auto-generated

### Loan Management
- Maximum 5 books per member
- Default loan period is 10 days
- Books can only be renewed once
- Overdue books cannot be renewed
- Book availability is automatically updated on checkout/return

### Fine Management
- Fines are calculated for overdue books
- Fines can be paid or remain unpaid
- Fine amounts are tracked per loan

## Database Schema

The system uses Entity Framework Core with the following main entities:

- **Book**: Books with metadata, availability, and location info
- **Member**: Library members with contact info and status
- **Loan**: Book checkout records with due dates and status
- **Reservation**: Book reservation records
- **Fine**: Fine records for overdue books
- **Author**: Book authors
- **Category**: Book categories
- **Publisher**: Book publishers
- **BookAuthor**: Many-to-many relationship between books and authors
- **BookCategory**: Many-to-many relationship between books and categories

## Getting Started

1. Ensure you have .NET 6.0 or later installed
2. Update the connection string in `appsettings.json`
3. Run the database migrations
4. Start the API project
5. Access Swagger UI at `/swagger` for API documentation

## Configuration

Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "LibraryManagementDB": "Server=your-server;Database=LibraryManagement;Trusted_Connection=true;"
  }
}
```

## Dependencies

- .NET 6.0
- Entity Framework Core
- ASP.NET Core Identity
- Swagger/OpenAPI 