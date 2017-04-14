## UITableView bindings design ##

### UITableView data handling overview ###

UITableView relies on UITableViewDataSource protocol to get it data. Usually client (controller) implement few methods from UITableViewDataSource which control UITableView appearance and operations. This design is similar to Android Adapter design but has few differencies

1. UITableViewDataSource has few optional members and table view change behavior based on presense or absense of these memebrs. For example to enable editing functionality one has to implement 'tableView:commitEditingStyle:forRowAtIndexPath:' method
2. UITableView data source has much more methods than Spinner or ListView adapter.
3. UITableView support editing scenarios which makes this binding two way.
4. UITableView operates in two modes - plain and grouped which has it own implications on how underlying data source should be structured.

### Data Binding Design ###

All of above makes one-size-fit-all binding implementation very difficult. Instead it is better to provide base class and then derive subclasses implementing most common UITableView data binding scenarios. For example we can have

1. PlainUITableViewDataSource - to bind plain collection of objects
2. GroupedUITableViewDataSource - to bing list of groups of objects (two level list)
3. PlainEditableUITableViewDataSource - same as PlainUITableViewDataSource but also support remove and reorder flow
... and so on.

Typical use case look like 

```
Bindings.Add(View.MyPlainTableDataSource(model.MyList));
//...
public class MyView : UIView {
	
	public IUITableViewBinding MyPlainTableDataSource(IList<MyData> list) {
		return new PlainUITableViewDataSource<MyData> {
			DataSource = list,
			CellFactory = MyDataCell,
			TableView = MyTableView
		};
	}

	UITableViewCell GetPeopleCell(UITableView tv, MyData data, int index)
	{
		const string reuse = "MY_DATA_CELL";
		var cell = tv.DequeueReusableCell(reuse) ?? new UITableViewCell(UITableViewCellStyle.Default, reuse);
		cell.TextLabel.Text = data.Title;
		return cell;
	}
}
```

GroupedUITableViewDataSource example:

```
Bindings.Add(View.GetGroupedPeopleListBinding(model.GroupedPersons));
//...
        public IUITableViewBinding GetPeopleListBinding(IList<IList<MyData>> groupedPeople)
        {
            return new GroupedUITableViewDataSource<MyGroup, MyData>
            {
                DataSource = groupedPeople,
                CellFactory = GetPeopleCell,
                HeaderFactory = GetGroupHeader,
                HeaderHeightFactory = GetGroupHeaderHeight,
                TableView = PeopleList
            };
        }

        float GetGroupHeaderHeight(UITableView tv, MyGroup group, int groupIndex)
        {
            return 30;
        }

        UIView GetGroupHeader(UITableView tv, MyGroup group, int groupIndex)
        {
            const int labelTag = 12345;
            const string reuse = "HEADER_VIEW";
            var view = tv.DequeueReusableHeaderFooterView(reuse) ?? new MyHeaderView();
            view.TextLabel.Text = group.Name;
            return view;
        }

        UITableViewCell GetPeopleCell(UITableView tv, MyData person, int groupIndex, int itemIndex)
        {
            const string reuse = "PERSON_CELL";
            var cell = tv.DequeueReusableCell(reuse) ?? new UITableViewCell(UITableViewCellStyle.Default, reuse);
            cell.TextLabel.Text = person.Name;
            return cell;
        }

```

### Out of scope ###

There are other extension points not covered by bindings:

1. Custom UITableViewDelegate can be used to customize table look and feel
2. Pull to refresh can be bound to view model command
3. Pagination can be covered by combination of view event, loading command and custom IList implementation bound to table view

Such scenarios might be complex to implement from scratch and it makes sense to provide helper classes. Such helpers are outside of scope of UITableView data bindings.