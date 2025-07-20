# Library Management API - Test Examples

## Quick Start Test Data

### 1. Create Publishers First

```bash
# Publisher 1
curl -X POST "https://localhost:7001/api/Publishers" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Addison-Wesley Professional",
    "address": "75 Arlington Street, Suite 300, Boston, MA 02116"
  }'

# Publisher 2
curl -X POST "https://localhost:7001/api/Publishers" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Prentice Hall",
    "address": "One Lake Street, Upper Saddle River, NJ 07458"
  }'

# Publisher 3
curl -X POST "https://localhost:7001/api/Publishers" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Manning Publications",
    "address": "20 Baldwin Road, Shelter Island, NY 11964"
  }'
```

### 2. Create Categories

```bash
# Programming Category
curl -X POST "https://localhost:7001/api/Categories" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Programming"
  }'

# Software Engineering Category
curl -X POST "https://localhost:7001/api/Categories" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Software Engineering"
  }'

# Design Patterns Category
curl -X POST "https://localhost:7001/api/Categories" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Design Patterns"
  }'

# C# Category
curl -X POST "https://localhost:7001/api/Categories" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "C#"
  }'
```

### 3. Create Authors

```bash
# Andrew Hunt
curl -X POST "https://localhost:7001/api/Authors" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Andrew",
    "lastName": "Hunt"
  }'

# David Thomas
curl -X POST "https://localhost:7001/api/Authors" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "David",
    "lastName": "Thomas"
  }'

# Robert Martin
curl -X POST "https://localhost:7001/api/Authors" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Robert",
    "lastName": "Martin"
  }'

# Jon Skeet
curl -X POST "https://localhost:7001/api/Authors" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Jon",
    "lastName": "Skeet"
  }'
```

### 4. Create Books

```bash
# Book 1: The Pragmatic Programmer
curl -X POST "https://localhost:7001/api/Books" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "The Pragmatic Programmer",
    "isbn": "978-0135957059",
    "publisherId": "REPLACE_WITH_PUBLISHER_ID_1",
    "description": "Your Journey to Mastery, 20th Anniversary Edition",
    "publicationDate": "2019-09-13",
    "language": "English",
    "totalCopies": 5,
    "availableCopies": 5,
    "rackNumber": "A1",
    "barcode": "BC001"
  }'

# Book 2: Clean Code
curl -X POST "https://localhost:7001/api/Books" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Clean Code: A Handbook of Agile Software Craftsmanship",
    "isbn": "978-0132350884",
    "publisherId": "REPLACE_WITH_PUBLISHER_ID_1",
    "description": "Even bad code can function. But if code is not clean, it can bring a development organization to its knees.",
    "publicationDate": "2008-08-11",
    "language": "English",
    "totalCopies": 3,
    "availableCopies": 3,
    "rackNumber": "A2",
    "barcode": "BC002"
  }'

# Book 3: C# in Depth
curl -X POST "https://localhost:7001/api/Books" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "C# in Depth",
    "isbn": "978-1617294532",
    "publisherId": "REPLACE_WITH_PUBLISHER_ID_3",
    "description": "Fourth Edition - Comprehensive guide to C#",
    "publicationDate": "2019-03-15",
    "language": "English",
    "totalCopies": 6,
    "availableCopies": 6,
    "rackNumber": "B1",
    "barcode": "BC005"
  }'
```

### 5. Create Members

```bash
# Member 1: John Doe
curl -X POST "https://localhost:7001/api/Members" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "phone": "555-0101",
    "address": "123 Main Street, Anytown, ST 12345",
    "role": "Member"
  }'

# Member 2: Jane Smith
curl -X POST "https://localhost:7001/api/Members" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Jane",
    "lastName": "Smith",
    "email": "jane.smith@example.com",
    "phone": "555-0102",
    "address": "456 Oak Avenue, Somewhere, ST 67890",
    "role": "Member"
  }'

# Member 3: Bob Johnson
curl -X POST "https://localhost:7001/api/Members" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Bob",
    "lastName": "Johnson",
    "email": "bob.johnson@example.com",
    "phone": "555-0103",
    "address": "789 Pine Road, Elsewhere, ST 11111",
    "role": "Member"
  }'
```

## Test API Functionality

### 1. Search Books

```bash
# Search by title
curl "https://localhost:7001/api/Books/search?title=pragmatic"

# Search by author
curl "https://localhost:7001/api/Books/search?author=robert"

# Search by category
curl "https://localhost:7001/api/Books/search?category=programming"
```

### 2. Check Member Eligibility

```bash
# Check if member can checkout (replace with actual member ID)
curl "https://localhost:7001/api/Members/REPLACE_WITH_MEMBER_ID/can-checkout"

# Get member's active loan count
curl "https://localhost:7001/api/Members/REPLACE_WITH_MEMBER_ID/active-loan-count"
```

### 3. Checkout a Book

```bash
# Checkout book (replace with actual IDs)
curl -X POST "https://localhost:7001/api/Loans/checkout" \
  -H "Content-Type: application/json" \
  -d '{
    "bookId": "REPLACE_WITH_BOOK_ID",
    "memberId": "REPLACE_WITH_MEMBER_ID"
  }'
```

### 4. Check Book Availability

```bash
# Check if book is available
curl "https://localhost:7001/api/Loans/book/REPLACE_WITH_BOOK_ID/available"
```

### 5. Get Member Loans

```bash
# Get all loans for a member
curl "https://localhost:7001/api/Members/REPLACE_WITH_MEMBER_ID/loans"
```

### 6. Return a Book

```bash
# Return book (replace with actual loan ID)
curl -X PUT "https://localhost:7001/api/Loans/REPLACE_WITH_LOAN_ID/return"
```

### 7. Renew a Book

```bash
# Renew book (replace with actual loan ID)
curl -X PUT "https://localhost:7001/api/Loans/REPLACE_WITH_LOAN_ID/renew"
```

## PowerShell Examples

If you prefer PowerShell, here are some examples:

```powershell
# Create a book
$bookData = @{
    title = "The Pragmatic Programmer"
    isbn = "978-0135957059"
    publisherId = "REPLACE_WITH_PUBLISHER_ID"
    description = "Your Journey to Mastery, 20th Anniversary Edition"
    publicationDate = "2019-09-13"
    language = "English"
    totalCopies = 5
    availableCopies = 5
    rackNumber = "A1"
    barcode = "BC001"
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://localhost:7001/api/Books" -Method POST -Body $bookData -ContentType "application/json"

# Create a member
$memberData = @{
    firstName = "John"
    lastName = "Doe"
    email = "john.doe@example.com"
    phone = "555-0101"
    address = "123 Main Street, Anytown, ST 12345"
    role = "Member"
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://localhost:7001/api/Members" -Method POST -Body $memberData -ContentType "application/json"

# Search books
$books = Invoke-RestMethod -Uri "https://localhost:7001/api/Books/search?title=pragmatic" -Method GET
$books | ConvertTo-Json -Depth 3
```

## Postman Collection

You can also import these requests into Postman:

1. **Create Book**
   - Method: POST
   - URL: `https://localhost:7001/api/Books`
   - Headers: `Content-Type: application/json`
   - Body: Use any of the book JSON examples above

2. **Create Member**
   - Method: POST
   - URL: `https://localhost:7001/api/Members`
   - Headers: `Content-Type: application/json`
   - Body: Use any of the member JSON examples above

3. **Search Books**
   - Method: GET
   - URL: `https://localhost:7001/api/Books/search?title=pragmatic`

4. **Checkout Book**
   - Method: POST
   - URL: `https://localhost:7001/api/Loans/checkout`
   - Headers: `Content-Type: application/json`
   - Body: `{"bookId": "GUID", "memberId": "GUID"}`

## Notes

- Replace `REPLACE_WITH_PUBLISHER_ID`, `REPLACE_WITH_BOOK_ID`, `REPLACE_WITH_MEMBER_ID`, etc. with actual GUIDs returned from your API calls
- The API will auto-generate library card numbers and barcodes for members
- Book availability is automatically managed when books are checked out or returned
- The system enforces a maximum of 5 books per member
- Loan periods are set to 10 days by default 