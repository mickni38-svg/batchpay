# BatchPay

**BatchPay** is a coordinated group payment system with social context — not just a simple bill-splitting app.

## 🎯 What BatchPay Really Is

BatchPay enables **coordinated group payments** where friends can organize and manage shared expenses together. The focus is on **coordination and social context**, not immediate money transfer.

### Core Philosophy

- **Friends-first approach**: All group payments require an existing social connection
- **Coordination over transaction**: The system manages who participates and what they owe, not actual bank transfers
- **Social context matters**: Each group payment has semantic meaning (pizza night, trip, drinks) represented through icons and titles

## 🔹 Core Features

### 1. Create Group Payments

Each group payment includes:
- **Title** — e.g., "Tony's pizza fredag aften" (Tony's pizza Friday night)
- **Message** — Additional context for participants
- **Icon** — Semantic visual identifier (🍕 pizza, ✈️ trip, 🍺 beer, etc.)
- **Selected participants** — Only from your friends list

### 2. Friends Are a Prerequisite (Social Graph)

- Users must first connect as friends before creating group payments together
- Friend management includes:
  - Search for users by name, email, or phone
  - Send and accept friend requests
  - View friends list with status (Pending/Accepted)

### 3. Group Payment Overview

The UI provides:
- **"Chips" / tabs** for each active group payment
- **Member accordion** per payment showing all participants
- **Pay / Deactivate** actions (soft delete — no actual money transfer yet)
- Status tracking for each member's participation

### 4. No Actual Money Transfer (Yet)

The current focus is on:
- ✅ Coordinating who participates in which group payment
- ✅ Tracking what each person owes
- ✅ Managing the social context around shared expenses
- ❌ Not yet: Bank integration or actual fund transfers

## 🏗️ Architecture

BatchPay follows a clean **MVVM (Model-View-ViewModel) / API-first architecture**:

### Project Structure

```
batchpay/
├── BatchPay/              # MAUI Frontend (Mobile-first)
│   ├── Pages/            # XAML UI pages
│   ├── ViewModels/       # MVVM view models
│   ├── Models/           # Frontend DTOs
│   └── FrontendServices/ # API communication layer
│
├── BatchPay.Api/         # ASP.NET Core REST API
│   ├── Controllers/      # API endpoints
│   ├── Services/         # Application services
│   └── Dtos/            # API data transfer objects
│
├── BatchPayLogic/        # Business Logic Layer
│   ├── ServiceLogic.cs  # Core business rules
│   ├── Interface/       # Service contracts
│   └── Dtos/            # Shared DTOs
│
└── Data/                 # Data Access Layer
    ├── Model/           # EF Core entities (User, FriendRequest)
    ├── Migrations/      # Database schema evolution
    └── BatchPayContext.cs # EF DbContext
```

### Technology Stack

- **Frontend**: .NET MAUI (cross-platform: iOS, Android, Windows, macOS)
- **Backend**: ASP.NET Core 8.0 Web API
- **Database**: Entity Framework Core with SQL Server
- **UI Pattern**: MVVM with CommunityToolkit.Mvvm
- **Architecture**: Clean architecture with separation of concerns

### Mobile-First, Web-Ready

- **Primary target**: Mobile devices (MAUI)
- **Backend design**: RESTful API suitable for any client (web, mobile, desktop)
- **Future-proof**: Easy to add web frontend or other clients

## 📱 ViewModels & Flows

### HomeViewModel
Navigation hub providing access to:
- Friends page
- Group payment creation
- Notifications

### VennerPageViewModel (Friends Page)
- Load and display friends list
- Search/filter all users in the system
- Send friend requests
- Manage existing friend connections

### GroupPaymentViewModel
- Load user's friends
- Select participants for the group payment
- Set title and context
- Create group order via API
- Display status (creating, success, error)

### NotificationsViewModel
- Show incoming friend requests
- Show sent friend requests with status
- Display group payment invitations (future enhancement)

## 🔐 Domain Models

### User
```csharp
public class User
{
    public int UserId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string AvatarUrl { get; set; }
    
    // Navigation properties
    public ICollection<FriendRequest> SentFriendRequests { get; set; }
    public ICollection<FriendRequest> ReceivedFriendRequests { get; set; }
}
```

### FriendRequest
```csharp
public class FriendRequest
{
    public int FriendRequestId { get; set; }
    public int RequesterId { get; set; }
    public int ReceiverId { get; set; }
    public string Status { get; set; } // Pending, Accepted, Rejected, Removed
    public DateTime CreatedAtUtc { get; set; }
    
    // Navigation properties
    public User Requester { get; set; }
    public User Receiver { get; set; }
}
```

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK or later
- Visual Studio 2022 (for MAUI development)
- SQL Server (LocalDB or full instance)

### Running the API

```bash
cd BatchPay.Api
dotnet restore
dotnet run
```

The API will be available at `https://localhost:5001` (or `http://localhost:5000`) with Swagger UI at `/swagger`.

### Running the MAUI App

1. Open `BatchPay.sln` in Visual Studio 2022
2. Set `BatchPay` as the startup project
3. Select your target platform (Android, iOS, Windows, macOS)
4. Press F5 to build and run

## 📋 Use Cases

### UC1 + UC2: Search User & Send Friend Request

**Flow:**
1. User navigates to "Venner" (Friends) tab
2. User searches by name, email, or phone
3. System searches database and displays results as cards
4. User clicks on a card to select a user
5. System creates a FriendRequest with Status = "Pending"
6. UI updates to show "Afventer..." (Pending) badge
7. Selected user appears in friends overview with Pending status

See detailed use case: [UC1_UC2/UC1_UC2.md](./UC1_UC2/UC1_UC2.md)

### Create Group Payment

**Flow:**
1. User navigates to "Group Payment" page
2. System loads user's friends
3. User enters title (e.g., "Pizza night")
4. User selects icon/category (pizza, trip, beer, etc.)
5. User selects friends to participate
6. User creates the group payment
7. System generates unique group code
8. Participants can view and manage their participation

## 🎨 UI/UX Highlights

- **Dark theme** with modern card-based design
- **Semantic icons** for quick recognition (🍕 pizza, ✈️ trip, 🍺 drinks)
- **Chips/tabs navigation** for switching between group payments
- **Accordion members list** for each payment
- **Real-time status updates** (pending, confirmed, paid, deactivated)

## 🔮 Future Enhancements

While currently focused on coordination, BatchPay is designed to support:
- [ ] Actual payment integration (MobilePay, bank transfers)
- [ ] Merchant integration (restaurants, venues)
- [ ] Push notifications for friend requests and payment updates
- [ ] Web client for desktop users
- [ ] Payment history and analytics
- [ ] Recurring group payments
- [ ] Split rules (equal, custom percentages, itemized)

## 📝 Contributing

This is a proof-of-concept (POC) demonstrating MVVM architecture and API-first design for coordinated group payments.

## 📄 License

[Add your license here]

---

**BatchPay** — Coordinated group payments with friends, not just bill splitting.
