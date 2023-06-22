export interface Column {
  key: string,
  display: (value: any) => string, // Defines how the displayed value is created
  value: any,
  queryable: boolean, // For searching the table
  editable: boolean, // For making values editable
  edit?: { // Settings for editable columns
    type: string, // Specifies the way the value can be edited
    disabled?: boolean, // Disables editing of the column based on a condition
    options?: {
      id: (value: any) => string,
      value?: any[], // Defines pre-defined options
      fetchUrl?: string, // Defines the URL options can be fetched from
      display: (value: any) => string,
      displaySub?: (value: any) => string, // Defines how the option's secondary display value is created
      queryable: (value: any) => string, // Defines how the option's queryable value is created
    }
  },
  create?: { // Settings for creatable columns
    value?: any, // Sets a default value
    disabled?: boolean // Hides the column when creating
  }
}

export interface INavSidebarItem {
  name: string,
  href: string,
  icon: string,
  subItems?: INavSidebarSubItem[],
  cb?(): void
}

export interface INavSidebarSubItem {
  name: string,
  href: string
}