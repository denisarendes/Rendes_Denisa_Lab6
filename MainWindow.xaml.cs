using AutoLotModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rendes_Denisa_Lab6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    enum ActionState 
    { 
        New,
        Edit,
        Delete,
        Nothing
    }

    public partial class MainWindow : Window
    {
        ActionState action = ActionState.Nothing;
        AutoLotEntitiesModel ctx = new AutoLotEntitiesModel();
        CollectionViewSource customerViewSource;
        CollectionViewSource inventoryViewSource;
        CollectionViewSource customerOrdersViewSource;

        //pt comunicarea cu tabelul Customer
        Binding firstNameTextBoxBinding = new Binding();
        Binding lastNameTextBoxBinding = new Binding();

        //pt comunicare cu tabelul Inventory
        Binding colorTextBoxBinding = new Binding();
        Binding makeTextBoxBinding = new Binding();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            //binding Customer
            firstNameTextBoxBinding.Path = new PropertyPath("FirstName");
            firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameTextBoxBinding);
            lastNameTextBoxBinding.Path = new PropertyPath("LastName");
            lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameTextBoxBinding);

            //binding Inventory
            colorTextBoxBinding.Path = new PropertyPath("Color");
            colorTextBox.SetBinding(TextBox.TextProperty, colorTextBoxBinding);
            makeTextBoxBinding.Path = new PropertyPath("Make");
            makeTextBox.SetBinding(TextBox.TextProperty, makeTextBoxBinding);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //using System.Data.Entity;
            customerViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));
            customerViewSource.Source = ctx.Customers.Local;
            ctx.Customers.Load();

            inventoryViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("inventoryViewSource")));
            inventoryViewSource.Source = ctx.Inventories.Local;
            ctx.Inventories.Load();

            customerOrdersViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerOrdersViewSource")));
            //customerOrdersViewSource.Source = ctx.Orders.Local;
            ctx.Orders.Load();

            cmbCustomers.ItemsSource = ctx.Customers.Local;
            //cmbCustomers.DisplayMemberPath = "FirstName";
            cmbCustomers.SelectedValuePath = "CustId";

            cmbInventory.ItemsSource = ctx.Inventories.Local;
            //cmbInventory.DisplayMemberPath = "Make";
            cmbInventory.SelectedValuePath = "CarId";
            
            BindDataGrid();
        }

        //event handlers Customer
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = null;
            if (action == ActionState.New)
            {
                try
                {
                    //instantare Customer entity
                    customer = new Customer()
                    {
                        FirstName = firstNameTextBox.Text.Trim(),
                        LastName = lastNameTextBox.Text.Trim()
                    };
                    SetValidationBinding();

                    //adaugam entitatea nou creata in context
                    ctx.Customers.Add(customer);
                    customerViewSource.View.Refresh();

                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerDataGrid.IsEnabled = true;
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnNext.IsEnabled = true;
                btnPrev.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
            }
            else if (action == ActionState.Edit)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    customer.FirstName = firstNameTextBox.Text.Trim();
                    customer.LastName = lastNameTextBox.Text.Trim();

                    SetValidationBinding();

                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerDataGrid.IsEnabled = true;
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnNext.IsEnabled = true;
                btnPrev.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
                customerViewSource.View.Refresh();
                //pozitionarea pe item-ul curent
                customerViewSource.View.MoveCurrentTo(customer);
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    ctx.Customers.Remove(customer);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerDataGrid.IsEnabled = true;
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnNext.IsEnabled = true;
                btnPrev.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
                customerViewSource.View.Refresh();
            }


        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToPrevious();
        }

        private void bntNext_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToNext();
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;

            customerDataGrid.IsEnabled = false;
            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            btnNext.IsEnabled = false;
            btnPrev.IsEnabled = false;
            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            firstNameTextBox.Text = "";
            lastNameTextBox.Text = "";
            Keyboard.Focus(firstNameTextBox);
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            string tempFirstName = firstNameTextBox.Text.ToString();
            string tempLastName = lastNameTextBox.Text.ToString();

            customerDataGrid.IsEnabled = false;
            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            btnNext.IsEnabled = false;
            btnPrev.IsEnabled = false;
            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            firstNameTextBox.Text = tempFirstName;
            lastNameTextBox.Text = tempLastName;
            Keyboard.Focus(firstNameTextBox);

            SetValidationBinding();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
            string tempFirstName = firstNameTextBox.Text.ToString();
            string tempLastName = lastNameTextBox.Text.ToString();

            customerDataGrid.IsEnabled = false;
            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            btnNext.IsEnabled = false;
            btnPrev.IsEnabled = false;

            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            firstNameTextBox.Text = tempFirstName;
            lastNameTextBox.Text = tempLastName;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;

            customerDataGrid.IsEnabled = true;
            btnNew.IsEnabled = true;
            btnEdit.IsEnabled = true;
            btnDelete.IsEnabled = true;
            btnSave.IsEnabled = false;
            btnCancel.IsEnabled = false;
            btnNext.IsEnabled = true;
            btnPrev.IsEnabled = true;
            firstNameTextBox.IsEnabled = false;
            lastNameTextBox.IsEnabled = false;

            firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameTextBoxBinding);
            lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameTextBoxBinding);
        }


        //event handlers Inventory
        private void btnNew2_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;

            inventoryDataGrid.IsEnabled = false;
            btnNew2.IsEnabled = false;
            btnEdit2.IsEnabled = false;
            btnDelete2.IsEnabled = false;
            btnSave2.IsEnabled = true;
            btnCancel2.IsEnabled = true;
            btnNext2.IsEnabled = false;
            btnPrev2.IsEnabled = false;
            colorTextBox.IsEnabled = true;
            makeTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);
            colorTextBox.Text = "";
            makeTextBox.Text = "";
            Keyboard.Focus(colorTextBox);
        }

        private void btnEdit2_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            string tempColor = colorTextBox.Text.ToString();
            string tempMake = makeTextBox.Text.ToString();

            inventoryDataGrid.IsEnabled = false;
            btnNew2.IsEnabled = false;
            btnEdit2.IsEnabled = false;
            btnDelete2.IsEnabled = false;
            btnSave2.IsEnabled = true;
            btnCancel2.IsEnabled = true;
            btnNext2.IsEnabled = false;
            btnPrev2.IsEnabled = false;
            colorTextBox.IsEnabled = true;
            makeTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);
            colorTextBox.Text = tempColor;
            makeTextBox.Text = tempMake;
            Keyboard.Focus(colorTextBox);
        }

        private void btnDelete2_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
            string tempColor = colorTextBox.Text.ToString();
            string tempMake = makeTextBox.Text.ToString();

            inventoryDataGrid.IsEnabled = false;
            btnNew2.IsEnabled = false;
            btnEdit2.IsEnabled = false;
            btnDelete2.IsEnabled = false;
            btnSave2.IsEnabled = true;
            btnCancel2.IsEnabled = true;
            btnNext2.IsEnabled = false;
            btnPrev2.IsEnabled = false;

            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);
            colorTextBox.Text = tempColor;
            makeTextBox.Text = tempMake;
        }

        private void btn2Save_Click(object sender, RoutedEventArgs e)
        {
            Inventory inventory = null;
            if (action == ActionState.New)
            {
                try
                {
                    //instantiere entitate Inventory 
                    inventory = new Inventory()
                    {
                        Color = colorTextBox.Text.Trim(),
                        Make = makeTextBox.Text.Trim()
                    };

                    //adaugare entitate creata in context
                    ctx.Inventories.Add(inventory);
                    inventoryViewSource.View.Refresh();

                    //salvare modificari
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                inventoryDataGrid.IsEnabled = true;
                btnNew2.IsEnabled = true;
                btnEdit2.IsEnabled = true;
                btnDelete2.IsEnabled = true;
                btnSave2.IsEnabled = false;
                btnCancel2.IsEnabled = false;
                btnNext2.IsEnabled = true;
                btnPrev2.IsEnabled = true;
                colorTextBox.IsEnabled = false;
                makeTextBox.IsEnabled = false;
            }
            else if (action == ActionState.Edit)
            {
                try
                {
                    inventory = (Inventory)inventoryDataGrid.SelectedItem;
                    inventory.Color = colorTextBox.Text.Trim();
                    inventory.Make = makeTextBox.Text.Trim();
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                inventoryViewSource.View.Refresh();
                //pozitionare pe item curent
                inventoryViewSource.View.MoveCurrentTo(inventory);

                inventoryDataGrid.IsEnabled = true;
                btnNew2.IsEnabled = true;
                btnEdit2.IsEnabled = true;
                btnDelete2.IsEnabled = true;
                btnSave2.IsEnabled = false;
                btnCancel2.IsEnabled = false;
                btnNext2.IsEnabled = true;
                btnPrev2.IsEnabled = true;
                colorTextBox.IsEnabled = false;
                makeTextBox.IsEnabled = false;
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    inventory = (Inventory)inventoryDataGrid.SelectedItem;
                    ctx.Inventories.Remove(inventory);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                inventoryViewSource.View.Refresh();

                inventoryDataGrid.IsEnabled = true;
                btnNew2.IsEnabled = true;
                btnEdit2.IsEnabled = true;
                btnDelete2.IsEnabled = true;
                btnSave2.IsEnabled = false;
                btnCancel2.IsEnabled = false;
                btnNext2.IsEnabled = true;
                btnPrev2.IsEnabled = true;
                colorTextBox.IsEnabled = false;
                makeTextBox.IsEnabled = false;
            }

        }

        private void btn2Cancel_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;

            inventoryDataGrid.IsEnabled = true;
            btnNew2.IsEnabled = true;
            btnEdit2.IsEnabled = true;
            btnDelete2.IsEnabled = true;
            btnSave2.IsEnabled = false;
            btnCancel2.IsEnabled = false;
            btnNext2.IsEnabled = true;
            btnPrev2.IsEnabled = true;
            colorTextBox.IsEnabled = false;
            makeTextBox.IsEnabled = false;

            colorTextBox.SetBinding(TextBox.TextProperty, colorTextBoxBinding);
            makeTextBox.SetBinding(TextBox.TextProperty, makeTextBoxBinding);
        }

        private void btnPrevious2_Click(object sender, RoutedEventArgs e)
        {
            inventoryViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNext2_Click(object sender, RoutedEventArgs e)
        {
            inventoryViewSource.View.MoveCurrentToNext();
        }


        //event handlers Orders

        private void bntNew3_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;

            ordersDataGrid.IsEnabled = false;
            btnNew3.IsEnabled = false;
            btnEdit3.IsEnabled = false;
            btnDelete3.IsEnabled = false;
            btnSave3.IsEnabled = true;
            btnCancel3.IsEnabled = true;
            btnNext3.IsEnabled = false;
            btnPrev3.IsEnabled = false;
            cmbCustomers.IsEnabled = true;
            cmbInventory.IsEnabled = true;

            cmbCustomers.SelectedItem = "";
            cmbInventory.SelectedItem = "";
        }

        private void bntEdit3_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            string tempCustId = cmbCustomers.SelectedItem.ToString();
            string tempCarId = cmbInventory.SelectedItem.ToString();

            ordersDataGrid.IsEnabled = false;
            btnNew3.IsEnabled = false;
            btnEdit3.IsEnabled = false;
            btnDelete3.IsEnabled = false;
            btnSave3.IsEnabled = true;
            btnCancel3.IsEnabled = true;
            btnNext3.IsEnabled = false;
            btnPrev3.IsEnabled = false;
            cmbCustomers.IsEnabled = true;
            cmbInventory.IsEnabled = true;

            custIdTextBox.Text = tempCustId;
            carIdTextBox.Text = tempCarId;
        }

        private void btnDelete3_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
            string tempCustId = cmbCustomers.SelectedItem.ToString();
            string tempCarId = cmbInventory.SelectedItem.ToString();

            ordersDataGrid.IsEnabled = false;
            btnNew3.IsEnabled = false;
            btnEdit3.IsEnabled = false;
            btnDelete3.IsEnabled = false;
            btnSave3.IsEnabled = true;
            btnCancel3.IsEnabled = true;
            btnNext3.IsEnabled = false;
            btnPrev3.IsEnabled = false;
            cmbCustomers.IsEnabled = true;
            cmbInventory.IsEnabled = true;

            custIdTextBox.Text = tempCustId;
            carIdTextBox.Text = tempCarId;
        }

        private void btnSave3_Click(object sender, RoutedEventArgs e)
        {
            Order order = null;
            if (action == ActionState.New)
            {
                try
                {
                    Customer customer = (Customer)cmbCustomers.SelectedItem;
                    Inventory inventory = (Inventory)cmbInventory.SelectedItem;

                    //instantiere entitate Order
                    order = new Order()
                    {
                        CustId = customer.CustId,
                        CarId = inventory.CarId
                    };

                    //adaugare entitate in context
                    ctx.Orders.Add(order);
                    customerOrdersViewSource.View.Refresh();

                    //salvare modificari
                    ctx.SaveChanges();
                    BindDataGrid();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                
                ordersDataGrid.IsEnabled = true;
                btnNew3.IsEnabled = true;
                btnEdit3.IsEnabled = true;
                btnDelete3.IsEnabled = true;
                btnSave3.IsEnabled = false;
                btnCancel3.IsEnabled = false;
                btnNext3.IsEnabled = true;
                btnPrev3.IsEnabled = true;
                cmbCustomers.IsEnabled = false;
                cmbInventory.IsEnabled = false;
            }
            else if (action == ActionState.Edit)
            {
                dynamic selectedOrder = ordersDataGrid.SelectedItem;
                try
                {
                    /*
                    Customer customer = (Customer)cmbCustomers.SelectedItem;
                    Inventory inventory = (Inventory)cmbInventory.SelectedItem;

                    order = (Order)ordersDataGrid.SelectedItem;
                    order.CustId = customer.CustId;
                    order.CarId = inventory.CarId;
                    ctx.SaveChanges(); 
                    */

                    int curr_id = selectedOrder.OrderId;
                    var editedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                    if (editedOrder != null) {
                        editedOrder.CustId = Int32.Parse(cmbCustomers.SelectedValue.ToString());
                        editedOrder.CarId = Convert.ToInt32(cmbInventory.SelectedValue.ToString());
                        //Se salveaza modificarile
                        ctx.SaveChanges();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                
                customerOrdersViewSource.View.Refresh();

                /*
                //pozitionare pe item curent
                customerOrdersViewSource.View.MoveCurrentTo(order);
                */

                BindDataGrid();
                //pozitionare pe item curent
                customerViewSource.View.MoveCurrentTo(selectedOrder);

                ordersDataGrid.IsEnabled = true;
                btnNew3.IsEnabled = true;
                btnEdit3.IsEnabled = true;
                btnDelete3.IsEnabled = true;
                btnSave3.IsEnabled = false;
                btnCancel3.IsEnabled = false;
                btnNext3.IsEnabled = true;
                btnPrev3.IsEnabled = true;
                cmbCustomers.IsEnabled = false;
                cmbInventory.IsEnabled = false;
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    dynamic selectedOrder = ordersDataGrid.SelectedItem;

                    /*
                    order = (Order)ordersDataGrid.SelectedItem;
                    ctx.Orders.Remove(order);
                    ctx.SaveChanges();
                    */

                    int curr_id = selectedOrder.OrderId;
                    var deletedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                    if (deletedOrder != null) {
                        ctx.Orders.Remove(deletedOrder);
                        ctx.SaveChanges();
                        MessageBox.Show("Order deleted successfully", "Message");
                        BindDataGrid();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                
                customerOrdersViewSource.View.Refresh();
                
                ordersDataGrid.IsEnabled = true;
                btnNew3.IsEnabled = true;
                btnEdit3.IsEnabled = true;
                btnDelete3.IsEnabled = true;
                btnSave3.IsEnabled = false;
                btnCancel3.IsEnabled = false;
                btnNext3.IsEnabled = true;
                btnPrev3.IsEnabled = true;
                cmbCustomers.IsEnabled = false;
                cmbInventory.IsEnabled = false;
            }
        }

        private void btnCancel3_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;

            ordersDataGrid.IsEnabled = true;
            btnNew3.IsEnabled = true;
            btnEdit3.IsEnabled = true;
            btnDelete3.IsEnabled = true;
            btnSave3.IsEnabled = false;
            btnCancel3.IsEnabled = false;
            btnNext3.IsEnabled = true;
            btnPrev3.IsEnabled = true;
            cmbCustomers.IsEnabled = false;
            cmbInventory.IsEnabled = false;
        }

        private void bntPrevious3_Click(object sender, RoutedEventArgs e)
        {
            customerOrdersViewSource.View.MoveCurrentToPrevious();
        }

        private void bntNext3_Click(object sender, RoutedEventArgs e)
        {
            customerOrdersViewSource.View.MoveCurrentToNext();
        }

        private void BindDataGrid()
        {
            var queryOrder = from ord in ctx.Orders
                             join cust in ctx.Customers on ord.CustId equals
                             cust.CustId
                             join inv in ctx.Inventories on ord.CarId
                             equals inv.CarId
                             select new
                             {
                                 ord.OrderId,
                                 ord.CarId,
                                 ord.CustId,
                                 cust.FirstName,
                                 cust.LastName,
                                 inv.Make,
                                 inv.Color
                             };
            customerOrdersViewSource.Source = queryOrder.ToList();
        }

        private void SetValidationBinding() {
            Binding firstNameValidationBinding = new Binding();
            firstNameValidationBinding.Source = customerViewSource;
            firstNameValidationBinding.Path = new PropertyPath("FirstName");
            firstNameValidationBinding.NotifyOnValidationError = true;
            firstNameValidationBinding.Mode = BindingMode.TwoWay;
            firstNameValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            //string required
            firstNameValidationBinding.ValidationRules.Add(new StringNotEmpty());
            firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameValidationBinding);

            Binding lastNameValidationBinding = new Binding();
            lastNameValidationBinding.Source = customerViewSource;
            lastNameValidationBinding.Path = new PropertyPath("LastName");
            lastNameValidationBinding.NotifyOnValidationError = true;
            lastNameValidationBinding.Mode = BindingMode.TwoWay;
            lastNameValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            //string min length validator
            lastNameValidationBinding.ValidationRules.Add(new StringMinLenghtValidator());
            lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameValidationBinding);    //setare binding nou
        }
    }
}
