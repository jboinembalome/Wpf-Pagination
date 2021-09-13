# Wpf-Pagination
These are examples of code to use pagination in WPF.

## Example:

- **1-UserControl and ViewModel**

> If you are using the MVVM pattern as well as MVVM Light. :smiley: (It is also possible to replace RelayCommand with Command if needed :+1:) 

**Files:**

    - View\Pagination.xaml
    - ViewModel\PaginationViewModel.cs
    - Interfaces\IPaginationService.cs
    - Services\PaginationService.cs

- **2-Custom Control**

> If you have a library of controls in a separate project. :sunglasses:	

**Files:**

    - CustomControl\PagingControl.cs
    - Enums\PageChanges.cs
    - Events\PageChangedEventArgs.cs
    - Themes\Generic.xaml

- **3-Custom CollectionView**

> If you want to separate the paging logic. In WPF applications, all collections are associated with a collection view by default. 
The idea is to inherit from the CollectionView class to handle the pagination. 
However, using a CollectionView in a ViewModel creates a strong coupling between the View and the ViewModel but this use case can be useful in a Code-Behind project. :slightly_smiling_face:

**Files:**

    - CustomControl\PagingCollectionView.cs
    - ViewModel\PaginationWithCollectionViewViewModel.cs

