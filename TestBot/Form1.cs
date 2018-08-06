using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.IO;
using System.Threading;

namespace TestBot
{
    public partial class Form1 : Form
    {
        IWebDriver Browser;
        public Form1()
        {
            InitializeComponent();
        }

        void ChechPriceUAH(List<IWebElement> Product)
        {         
            richTextBox1.Text += "Check Currency & products (UAH)\n";
            //LogFile("Check Currency & products (UAH)", true);
            for (int i = 0; i < Product.Count; i++)
            {
                IWebElement Price = Product[i].FindElement(By.ClassName("price"));
                string[] buf = Price.Text.Split(new char[] {' '});
                if (buf[1] == "₴")
                {
                    richTextBox1.Text += (Product[i].FindElement(By.TagName("h1")).Text + " Price is OK.\n");
                }
                if (buf[1] != "₴")
                {
                    richTextBox1.Text +=(Product[i].FindElement(By.TagName("h1")).Text + " Price is Wrong.\n");
                }
            }
        }
        void ChechPriceEUR(List<IWebElement> Product)
        {
            richTextBox1.Text +="Check Currency & products (EUR)\n";
            for (int i = 0; i < Product.Count; i++)
            {
                IWebElement Price = Product[i].FindElement(By.ClassName("price"));
                string[] buf = Price.Text.Split(new char[] {' '});
                if (buf[1] == "€")
                {
                    richTextBox1.Text +=(Product[i].FindElement(By.TagName("h1")).Text + " Price is OK.\n");
                }
                if (buf[1] != "€")
                {
                    richTextBox1.Text +=(Product[i].FindElement(By.TagName("h1")).Text + " Price is Wrong.\n");
                }
            }
        }
        void ChechPriceUSD(List<IWebElement> Product)
        {
            richTextBox1.Text += ("Check Currency & products (USD)\n");
            for (int i = 0; i < Product.Count; i++)
            {
                IWebElement Price = Product[i].FindElement(By.ClassName("price"));
                string[] buf = Price.Text.Split(new char[] {' '});
                if (buf[1] == "$")
                {
                   richTextBox1.Text +=(Product[i].FindElement(By.TagName("h1")).Text + " Price is OK.\n");
                }
                if (buf[1] != "$")
                {
                    richTextBox1.Text += (Product[i].FindElement(By.TagName("h1")).Text + " Price is Wrong.\n");
                }
            }
        }

        int ChechRealAnsw(List<IWebElement> Product)
        {
            int A = 0;
            for(int i=0;i<Product.Count;i++)
            {
                IWebElement  RealProd = Product[i].FindElement(By.TagName("h1"));
                //IWebElement RealProd = Prod.FindElement(By.TagName("a"));
                string[] buf = RealProd.Text.Split(new char[] { ' ' });
                for(int j=0;j<buf.Length;j++)
                {
                    if (buf[j] == "dress" || buf[j] == "Dress")
                    {
                        A++;
                    }
                }
            }
            return (A);
        }
        bool CheckSort(List<IWebElement> Product)
        {
            bool tf = true;
            double [] A = new double[Product.Count];
            for(int i=0;i<Product.Count;i++)
            {
                IWebElement Price = Product[i].FindElement(By.ClassName("price"));
                 string[] buf = Price.Text.Split(new char[] { ' ' });
                 A[i] = Convert.ToDouble(buf[0]); 
            }
            double[] B = A;
            Array.Sort(B);
            for (int i = 0; i < Product.Count; i++)
            {
                if (A[i] != B[i]) tf = false;
            }
            return (tf);
        }
        bool CheckSale(List<IWebElement> Product)
        {
            bool tf = true;
            double[,] A = new double[Product.Count,3];
            for (int i = 0; i < Product.Count; i++)
            {
                IWebElement buff1 = Product[i].FindElement(By.ClassName("product-price-and-shipping"));
                List<IWebElement> Buff = buff1.FindElements(By.ClassName("span")).ToList();
                if(Buff.Count>2)
                {
                    IWebElement RegPrice = Product[i].FindElement(By.ClassName("regular-price"));
                    IWebElement Price = Product[i].FindElement(By.ClassName("price"));
                    IWebElement Discount = Product[i].FindElement(By.ClassName("discount-percentage"));
                    string[] buf = Price.Text.Split(new char[] { ' ' });
                    A[i, 0] = Convert.ToDouble(buf[0]);
                    buf = RegPrice.Text.Split(new char[] { ' ' });
                    A[i, 1] = Convert.ToDouble(buf[0]);
                    buf = Discount.Text.Split(new char[] { '%' });
                    A[i, 2] = Convert.ToDouble(buf[0]);
                }
                
            }
            for (int i = 0; i < Product.Count; i++)
            {
                if ((A[i, 1] + A[i, 1] * (A[i, 2] / 100)) != A[i, 0]) tf = false;
            }
            return (tf);
        }

        
        private void button1_Click(object sender, EventArgs e)
        {
            //Browser = new OpenQA.Selenium.Chrome.ChromeDriver();
            var optn = new ChromeOptions();
            var service = ChromeDriverService.CreateDefaultService();
            service.LogPath = "chromedriver.log";
            service.EnableVerboseLogging = true;
            Browser = new ChromeDriver(service, optn);
            try
            {    
                Browser.Navigate().GoToUrl("http://prestashop-automation.qatestlab.com.ua/ru/");
                richTextBox1.Text += ("Connecting to http://prestashop-automation.qatestlab.com.ua/ru/ ... \n");
            }
            catch(Exception)
            {
                richTextBox1.Text += ("URL http://prestashop-automation.qatestlab.com.ua/ru/ doesn't respond!\n");
                Browser.Close();
            }
            IWebElement Currency_selector = Browser.FindElement(By.Id("_desktop_currency_selector"));
            List<IWebElement> CurrencyA = Currency_selector.FindElements(By.TagName("span")).ToList();
            IWebElement CurrencyField = Currency_selector.FindElement(By.TagName("ul"));
            List<IWebElement> CurrencyList = CurrencyField.FindElements(By.TagName("li")).ToList();
            IWebElement ProductsField = Browser.FindElement(By.ClassName("products"));
            List<IWebElement> Product = ProductsField.FindElements(By.TagName("article")).ToList();
            for (int i = 0; i < CurrencyList.Count; i++)
            {
                CurrencyA[1].Click();
                Thread.Sleep(1000);
                CurrencyList[i].Click();
                Currency_selector = Browser.FindElement(By.Id("_desktop_currency_selector"));               
                CurrencyA = Currency_selector.FindElements(By.TagName("span")).ToList();
                CurrencyField = Currency_selector.FindElement(By.TagName("ul"));
                CurrencyList = CurrencyField.FindElements(By.TagName("li")).ToList();
                ProductsField = Browser.FindElement(By.ClassName("products"));
                Product = ProductsField.FindElements(By.TagName("article")).ToList();
                if (i==1)
                {
                    ChechPriceUAH(Product);
                }
                if (i==0)
                {
                    ChechPriceEUR(Product);
                }
                if (i==2)
                {
                    ChechPriceUSD(Product);
                }
            }
            richTextBox1.Text += ("Switch to USD price...\n");
            CurrencyA[1].Click();
            Thread.Sleep(1000);
            CurrencyList[2].Click();
            IWebElement SearchField = Browser.FindElement(By.Id("search_widget"));
            IWebElement SearchText =  SearchField.FindElement(By.Name("s"));
            IWebElement SearchButton = SearchField.FindElement(By.TagName("button"));

            SearchText.Click();
            SearchText.SendKeys("dress");
            Thread.Sleep(500);
            richTextBox1.Text += "Search for key (dress)\n";
            SearchButton.Click();
            Thread.Sleep(500);
            richTextBox1.Text += "Check products for key (dress)\n";
            ProductsField = Browser.FindElement(By.Id("products"));
            Product = ProductsField.FindElements(By.TagName("article")).ToList();
            int B = 0;
            B = ChechRealAnsw(Product);
            IWebElement Count = ProductsField.FindElement(By.TagName("p"));
            string[] buf = Count.Text.Split(new char[] { ' ' });
            char[] buf2 = buf[1].ToArray();
            if (Convert.ToInt32(buf2[0].ToString()) == B) richTextBox1.Text += ("Find elements:" + Product.Count.ToString() + " is correct!\n");
            else richTextBox1.Text += ("Find elements:" + B.ToString() + " not correct!\n");
            ChechPriceUSD(Product);
            Count = ProductsField.FindElement(By.ClassName("select-title"));
            Count.Click();
            Count = ProductsField.FindElement(By.ClassName("dropdown-menu"));
            List<IWebElement> DropDownMenu = Count.FindElements(By.TagName("a")).ToList();
            DropDownMenu[4].Click();
            Thread.Sleep(1000);
            ProductsField = Browser.FindElement(By.Id("products"));
            Product = ProductsField.FindElements(By.TagName("article")).ToList();
            richTextBox1.Text += ("Chech sorting...\n");
            if(CheckSort(Product) == true)
            {
                richTextBox1.Text += ("Sorting is correct!\n");
            }
            else
            {
                richTextBox1.Text += ("Sorting is not correct!\n");
            }
            richTextBox1.Text += ("Chech discount...\n");
            if (CheckSale(Product) == true)
            {
                richTextBox1.Text += ("Discount is correct!\n");
            }
            else
            {
                richTextBox1.Text += ("Discount is not correct!\n");
            }
            richTextBox1.Text += ("End of test!");
            Browser.Close();
        }
    }
}
