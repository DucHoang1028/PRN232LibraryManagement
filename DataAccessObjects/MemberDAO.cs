using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects
{
    public class MemberDAO
    {
        public static List<Member> GetMembers()
        {
            try
            {
                using var context = new ApplicationDbContext();
                return context.Members
                    .Include(m => m.Loans)
                    .Include(m => m.Reservations)
                    .Include(m => m.Fines)
                    .Where(m => m.IsActive)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static Member? GetMemberById(Guid memberId)
        {
            try
            {
                using var context = new ApplicationDbContext();
                return context.Members
                    .Include(m => m.Loans)
                    .Include(m => m.Reservations)
                    .Include(m => m.Fines)
                    .FirstOrDefault(m => m.MemberId == memberId && m.IsActive);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static Member? GetMemberByEmail(string email)
        {
            try
            {
                using var context = new ApplicationDbContext();
                return context.Members
                    .Include(m => m.Loans)
                    .Include(m => m.Reservations)
                    .Include(m => m.Fines)
                    .FirstOrDefault(m => m.Email == email && m.IsActive);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static Member? GetMemberByLibraryCard(string libraryCardNumber)
        {
            try
            {
                using var context = new ApplicationDbContext();
                return context.Members
                    .Include(m => m.Loans)
                    .Include(m => m.Reservations)
                    .Include(m => m.Fines)
                    .FirstOrDefault(m => m.LibraryCardNumber == libraryCardNumber && m.IsActive);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static Member CreateMember(Member member)
        {
            try
            {
                using var context = new ApplicationDbContext();
                member.MemberId = Guid.NewGuid();
                member.CreatedDate = DateTime.UtcNow;
                member.JoinDate = DateTime.UtcNow;
                member.IsActive = true;
                
                if (string.IsNullOrEmpty(member.LibraryCardNumber))
                    member.LibraryCardNumber = GenerateLibraryCardNumber();
                
                if (string.IsNullOrEmpty(member.Barcode))
                    member.Barcode = GenerateBarcode();

                context.Members.Add(member);
                context.SaveChanges();
                return member;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static Member UpdateMember(Guid memberId, Member member)
        {
            try
            {
                using var context = new ApplicationDbContext();
                var existingMember = context.Members.Find(memberId);
                if (existingMember == null)
                    throw new Exception("Member not found");

                existingMember.FirstName = member.FirstName;
                existingMember.LastName = member.LastName;
                existingMember.Email = member.Email;
                existingMember.Phone = member.Phone;
                existingMember.Address = member.Address;
                existingMember.Role = member.Role;
                existingMember.UpdatedDate = DateTime.UtcNow;

                context.SaveChanges();
                return existingMember;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static bool DeleteMember(Guid memberId)
        {
            try
            {
                using var context = new ApplicationDbContext();
                var member = context.Members.Find(memberId);
                if (member == null)
                    return false;

                member.IsActive = false;
                member.UpdatedDate = DateTime.UtcNow;
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static bool DeactivateMember(Guid memberId)
        {
            return DeleteMember(memberId);
        }

        public static List<Loan> GetMemberLoans(Guid memberId)
        {
            try
            {
                using var context = new ApplicationDbContext();
                return context.Loans
                    .Include(l => l.Book)
                    .Include(l => l.Fine)
                    .Where(l => l.MemberId == memberId)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static List<Reservation> GetMemberReservations(Guid memberId)
        {
            try
            {
                using var context = new ApplicationDbContext();
                return context.Reservations
                    .Include(r => r.Book)
                    .Where(r => r.MemberId == memberId)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static List<Fine> GetMemberFines(Guid memberId)
        {
            try
            {
                using var context = new ApplicationDbContext();
                return context.Fines
                    .Include(f => f.Loan)
                    .Where(f => f.MemberId == memberId)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static int GetActiveLoanCount(Guid memberId)
        {
            try
            {
                using var context = new ApplicationDbContext();
                return context.Loans
                    .Count(l => l.MemberId == memberId && l.Status == "Active");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static string GenerateLibraryCardNumber()
        {
            return $"LC{DateTime.Now:yyyyMMdd}{new Random().Next(1000, 9999)}";
        }

        private static string GenerateBarcode()
        {
            return $"BC{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(100, 999)}";
        }
    }
} 