﻿using c4_LocalDatabaseConnection.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace c4_LocalDatabaseConnection.ViewModels {
    public partial class MainViewModel : ObservableObject {
        [ObservableProperty]
        ObservableCollection<Customer> customers;

        [ObservableProperty]
        bool refreshing;

        [RelayCommand]
        void Showing() {
            Refreshing = true;
        }

        [RelayCommand]
        async Task LoadCustomersAsync() {
            using var uniOfWork = new CrmUnitOfWork();
            Customers = new ObservableCollection<Customer>(await uniOfWork.Items.GetAllAsync());
            Refreshing = false;
        }

        [RelayCommand]
        async Task ShowNewFormAsync() {
            await Shell.Current.GoToAsync(nameof(CustomerEditPage),
                parameters: new Dictionary<string, object>
                {
                        { "ParentRefreshAction", (Func<Customer, Task>)RefreshAddedAsync },
                        { "Item", new Customer() },
                        { "IsNewItem", true }
                });
        }

        Task RefreshAddedAsync(Customer addedCustomer) {
            Customers.Add(addedCustomer);
            return Task.CompletedTask;
        }
        [RelayCommand]
        async Task DeleteCustomerAsync(Customer customer) {
            using var uniOfWork = new CrmUnitOfWork();
            await uniOfWork.Items.DeleteAsync(customer);
            try {
                await uniOfWork.SaveAsync();
            }
            catch (Exception ex) {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
                return;
            }
            Customers.Remove(customer);
        }


        [RelayCommand]
        async Task ShowDetailFormAsync(Customer customer) {
            await Shell.Current.GoToAsync(nameof(CustomerDetailPage),
                    parameters: new Dictionary<string, object>
                    {
                        { "ParentRefreshAction", (Func<Customer, Task>)RefreshEditedAsync },
                        { "Item", customer }
                    });
        }

        async Task RefreshEditedAsync(Customer updatedCustomer) {
            int editedItemIndex = -1;
            await Task.Run(() =>
            {
                editedItemIndex = Customers.Select((customer, index) => new { customer, index }).
                                            First(item => item.customer.Id == updatedCustomer.Id).index;
            });
            if (editedItemIndex == -1)
                return;
            Customers[editedItemIndex] = updatedCustomer;
        }
    }

    public class CrmContext : DbContext {
        public DbSet<Customer> Customers { get; set; }
        public CrmContext() {
            SQLitePCL.Batteries_V2.Init();
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "localdatabase.db");
            optionsBuilder.UseSqlite($"Filename={dbPath}");
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder builder) {
            builder.Entity<Customer>()
                .HasIndex(c => c.Email)
                .IsUnique();
        }
    }
}
