using System;
using System.ComponentModel;

namespace library_management_system.Models
{
    public class Loan : INotifyPropertyChanged
    {
        private int _id;
        private int _bookId;
        private int _memberId;
        private DateTime _loanDate;
        private DateTime _dueDate;
        private DateTime? _returnDate;
        private bool _isReturned;
        private decimal _fineAmount;

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public int BookId
        {
            get => _bookId;
            set
            {
                _bookId = value;
                OnPropertyChanged(nameof(BookId));
            }
        }

        public int MemberId
        {
            get => _memberId;
            set
            {
                _memberId = value;
                OnPropertyChanged(nameof(MemberId));
            }
        }

        public DateTime LoanDate
        {
            get => _loanDate;
            set
            {
                _loanDate = value;
                OnPropertyChanged(nameof(LoanDate));
            }
        }

        public DateTime DueDate
        {
            get => _dueDate;
            set
            {
                _dueDate = value;
                OnPropertyChanged(nameof(DueDate));
            }
        }

        public DateTime? ReturnDate
        {
            get => _returnDate;
            set
            {
                _returnDate = value;
                OnPropertyChanged(nameof(ReturnDate));
            }
        }

        public bool IsReturned
        {
            get => _isReturned;
            set
            {
                _isReturned = value;
                OnPropertyChanged(nameof(IsReturned));
            }
        }

        public decimal FineAmount
        {
            get => _fineAmount;
            set
            {
                _fineAmount = value;
                OnPropertyChanged(nameof(FineAmount));
            }
        }

        // Navigation properties (for display purposes)
        public Book Book { get; set; }
        public Member Member { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 