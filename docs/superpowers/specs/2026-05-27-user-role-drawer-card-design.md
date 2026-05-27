# Design: User Role Card in Drawer

**Date:** 2026-05-27  
**Status:** Approved

## Summary

Add a user info card at the bottom of the left sidebar drawer in `RootLayout`. The card collapses to an icon when the drawer is closed and expands to show full user info when the drawer is open.

## Placement

- Inside the existing `Drawer` component in `RootLayout.jsx`
- Below the second `<Divider>` at the bottom of the nav list
- Sticky to the bottom using `Box sx={{ mt: 'auto' }}`

## New File

`src/components/layout/UserDrawerCard.jsx`

## Props

| Prop | Type | Description |
|------|------|-------------|
| `open` | boolean | Drawer open state (passed from RootLayout) |
| `user` | object | From `useAuth()`: `nombreCompleto`, `email`, `rol` |

## Behavior

### Drawer closed (`open=false`)
- Renders a centered `IconButton` with `AccountCircleIcon`
- `Tooltip` on hover shows the role label (e.g., "Admin")
- No text visible

### Drawer open (`open=true`)
- Renders a `Box` with `px: 2, py: 1.5` padding
- Row 1: `Typography variant="body2"` bold, `noWrap` — user's full name
- Row 2: `Typography variant="caption"` color `text.secondary`, `noWrap` — email
- Row 3: `Chip` size="small" with role label and color:

| Rol | Valor | Chip color |
|-----|-------|------------|
| Free | 0 | `default` (grey) |
| Premium | 1 | `warning` (orange) |
| Admin | 2 | `primary` (blue) |
| Invitado | 3 | `success` (green) |

### Role label mapping (local to component)
```js
const ROL_LABELS = { 0: 'Free', 1: 'Premium', 2: 'Admin', 3: 'Invitado' };
const ROL_COLORS = { 0: 'default', 1: 'warning', 2: 'primary', 3: 'success' };
```

## Transition

The collapsed→expanded transition follows the same `opacity: open ? 1 : 0` pattern already used by `ListItemText` components in the drawer. No additional animation needed.

## Changes to RootLayout.jsx

1. Add `user` to the `useAuth()` destructure: `const { logout, canCreate, isAdmin, user } = useAuth();`
2. Import `UserDrawerCard`
3. Add `<UserDrawerCard open={open} user={user} />` after the second `<Divider>` inside the `Drawer`

## Error Handling

If `user` is `null`, `UserDrawerCard` returns `null` (renders nothing). This should not occur in practice since the drawer is only rendered when `showSidebar=true` (authenticated routes), but the guard prevents runtime errors.

## Scope

- No new context, no new state, no API calls
- Single new component file + 3-line change in RootLayout
